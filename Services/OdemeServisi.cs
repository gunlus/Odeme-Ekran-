using System;
using System.Data.Common;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Odeme_Projesi.Data;
using Odeme_Projesi.Models;

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
        try
        {
            // Ödemeyi ve ilişkili verileri getir (Include ile)
            var odeme = _db.Odemeler
                .Include(o => o.AlacakliHesap)
                    .ThenInclude(h => h.Musteri) // Müşteri bilgisine erişmek için
                .FirstOrDefault(o => o.Id == odemeId);

            if (odeme == null)
                return (false, "Ödeme emri bulunamadı!");

            if (odeme.OdemeDurumu == true)
                return (false, "Bu ödeme zaten gerçekleştirilmiş!");

            // Alacaklı hesabı kontrol et
            var alacakliHesap = odeme.AlacakliHesap;
            if (alacakliHesap == null)
                return (false, "Alacaklı hesap bulunamadı!");

            // Bakiye güncellemeleri
            decimal bakiyeOnce = alacakliHesap.Bakiye;
            alacakliHesap.KumuleAlacakArttir(odeme.OdemeMiktari);
            decimal bakiyeSonra = alacakliHesap.Bakiye;

            // Ödeme durumunu güncelle
            odeme.OdemeDurumu = true;
            odeme.SonOdemeTarihi = DateTime.Now;

            // Muhasebe Defteri'ne kayıt ekle (HesapId ile)
            var muhasebeKayit = new MuhasebeDefteri
            {
                OdemeId = odeme.Id,
                HesapId = alacakliHesap.Id,
                Tutar = odeme.OdemeMiktari,
                BakiyeOnce = bakiyeOnce,
                BakiyeSonra = bakiyeSonra,
                Tarih = DateTime.Now,
                IslemTipi = "ÖDEME",
                Aciklama = odeme.OdemeAciklamasi
            };

            _db.MuhasebeDefteri.Add(muhasebeKayit);
            _db.SaveChanges();

            _log.Bilgi($"Muhasebe kaydı eklendi: Ödeme ID={odeme.Id}, Tutar={odeme.OdemeMiktari:C}, Hesap={alacakliHesap.HesapNo}", "OdemeYap");

            return (true, $"✅ {odeme.OdemeMiktari:C} , {alacakliHesap.Musteri.TCKN} hesabına başarıyla ödendi!");
        }
        catch (Exception ex)
        {
            _log.Hata($"Ödeme hatası ID:{odemeId} - {ex.Message}", "OdemeYap");
            return (false, $"Hata: {ex.Message}");
        }
    }
}