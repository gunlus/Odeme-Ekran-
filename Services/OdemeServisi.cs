using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Odeme_Projesi.Data;
using Odeme_Projesi.Models;
using System.Collections.Generic;
namespace Odeme_Projesi.Services;

public class OdemeServisi
{
    private readonly BankaDbContext _db;
    private readonly LogServisi _log;

    public OdemeServisi(BankaDbContext db)
    {
        _db = db;
        _log = new LogServisi();
    }

    public (bool basarili, string mesaj) OdemeYap(int odemeId)
    {
        using var transaction = _db.Database.BeginTransaction();
        try
        {
            // 1. Banka müşterisini bul veya oluştur
            var bankaMusteri = _db.Musteriler.FirstOrDefault(m => m.TCKN == "00000000000");
            if (bankaMusteri == null)
            {
                bankaMusteri = new Musteri("00000000000", "BANKA", "HESAP");
                _db.Musteriler.Add(bankaMusteri);
                _db.SaveChanges(); // ID atansın
            }

            // 2. Banka hesabını bul veya oluştur (MusteriId artık geçerli!)
            var bankaHesap = _db.Hesaplar.FirstOrDefault(h => h.HesapNo == "BANKA0001");
            if (bankaHesap == null)
            {
                bankaHesap = new Hesap("BANKA0001", bankaMusteri.Id, 1_000_000, 0);
                _db.Hesaplar.Add(bankaHesap);
                _db.SaveChanges(); // ID atansın
            }

            // 3. Ödemeyi getir (Include ile)
            var odeme = _db.Odemeler
                .Include(o => o.AlacakliHesap)
                    .ThenInclude(h => h.Musteri)
                .FirstOrDefault(o => o.Id == odemeId);

            if (odeme == null)
                return (false, "Ödeme emri bulunamadı!");

            if (odeme.OdemeDurumu == true)
                return (false, "Bu ödeme zaten gerçekleştirilmiş!");

            var alacakliHesap = odeme.AlacakliHesap;
            if (alacakliHesap == null)
                return (false, "Alacaklı hesap bulunamadı!");

            // 4. ALACAKLI HESAP (Müşteri)
            decimal alacakliBakiyeOnce = alacakliHesap.Bakiye;
            alacakliHesap.KumuleAlacakArttir(odeme.OdemeMiktari);
            decimal alacakliBakiyeSonra = alacakliHesap.Bakiye;

            // 5. BORÇLU HESAP (Banka)
            decimal borcluBakiyeOnce = bankaHesap.Bakiye;
            bool borcGuncellendi = bankaHesap.KumuleBorcArttir(odeme.OdemeMiktari);
            if (!borcGuncellendi)
                return (false, "Banka hesabında yeterli bakiye yok!");
            decimal borcluBakiyeSonra = bankaHesap.Bakiye;

            // 6. ÖDEME DURUMUNU GÜNCELLE
            odeme.OdemeDurumu = true;
            odeme.SonOdemeTarihi = DateTime.Now;

            // 7. MUHASEBE KAYITLARI (Çift taraflı)
            var alacakKayit = new MuhasebeDefteri
            {
                OdemeId = odeme.Id,
                HesapId = alacakliHesap.Id,
                Tutar = odeme.OdemeMiktari,
                BakiyeOnce = alacakliBakiyeOnce,
                BakiyeSonra = alacakliBakiyeSonra,
                Tarih = DateTime.Now,
                IslemTipi = "ALACAK",
                Aciklama = odeme.OdemeAciklamasi + " (Müşteri)"
            };
            _db.MuhasebeDefteri.Add(alacakKayit);

            var borcKayit = new MuhasebeDefteri
            {
                OdemeId = odeme.Id,
                HesapId = bankaHesap.Id,
                Tutar = odeme.OdemeMiktari,
                BakiyeOnce = borcluBakiyeOnce,
                BakiyeSonra = borcluBakiyeSonra,
                Tarih = DateTime.Now,
                IslemTipi = "BORÇ",
                Aciklama = odeme.OdemeAciklamasi + " (Banka)"
            };
            _db.MuhasebeDefteri.Add(borcKayit);

            // 8. TÜM DEĞİŞİKLİKLERİ KAYDET
            _db.SaveChanges();
            transaction.Commit();

            _log.Bilgi($"Ödeme başarılı. ID={odeme.Id}, Tutar={odeme.OdemeMiktari:C}", "OdemeYap");
            return (true, $"✅ {odeme.OdemeMiktari:C} başarıyla ödendi!");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _log.Hata($"Ödeme hatası ID:{odemeId} - {ex.Message}", "OdemeYap");
            return (false, $"Hata: {ex.Message}");
        }
    }
    
    // OdemeServisi.cs

    public Musteri? MusteriVeHesaplariGetir(string tckn)
    {
        return _db.Musteriler
            .Include(m => m.Hesaplar)
            .FirstOrDefault(m => m.TCKN == tckn);
    }

    public List<Odeme> BekleyenOdemeleriGetir(int hesapId)
    {
        return _db.Odemeler
            .Include(o => o.AlacakliHesap)
            .Where(o => o.AlacakliHesapId == hesapId && o.OdemeDurumu == false)
            .ToList();
    }
}