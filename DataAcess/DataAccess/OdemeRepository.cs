using Entities;
namespace DataAccess;

using Microsoft.EntityFrameworkCore;

public class OdemeRepository : IOdemeRepository
{
    private readonly BankaDbContext _db;

    public OdemeRepository(BankaDbContext db)
    {
        _db = db;
    }

    public (bool basarili, string mesaj) OdemeIsleminiGerceklestir(int odemeId)
    {
        using var transaction = _db.Database.BeginTransaction();
        try
        {
            // Ödeme, alacaklı hesap ve borçlu hesap (sistem hesabı) ile birlikte getiriliyor
            var odeme = _db.Odemeler
                .Include(o => o.AlacakliHesap)
                .Include(o => o.BorcluHesap)
                .FirstOrDefault(o => o.Id == odemeId);

            if (odeme == null || odeme.OdemeDurumu == true)
                return (false, "Ödeme bulunamadı veya zaten yapılmış!");

            // Eğer borçlu hesap atanmamışsa sistem hesabını bulup atayalım
            if (odeme.BorcluHesapId == null)
            {
                var bankaHesap = _db.Hesaplar.FirstOrDefault(h => h.HesapNo == "BANKA0001");
                if (bankaHesap == null) return (false, "Sistem banka hesabı bulunamadı!");
                
                odeme.BorcluHesapId = bankaHesap.Id;
                odeme.BorcluHesap = bankaHesap;
            }

            // 1. İşlem Öncesi Net Bakiyeler Kaydediliyor (Bankanın net bakiye havuzu baz alınıyor)
            decimal alacakliBakiyeOnce = odeme.AlacakliHesap.Bakiye;
            decimal borcluBakiyeOnce = odeme.BorcluHesap.Bakiye;

            // 2. Bakiyeler Güncelleniyor
            odeme.AlacakliHesap.KumuleAlacakArttir(odeme.OdemeMiktari);
            odeme.BorcluHesap.KumuleBorcArttir(odeme.OdemeMiktari);
            
            odeme.OdemeDurumu = true;

            // 3. MUHASEBE DEFTERİ: Alacak Kaydı (Müşteri Hesabı İçin)
            var alacakKaydi = new MuhasebeDefteri
            {
                OdemeId = odeme.Id,
                HesapId = odeme.AlacakliHesapId,
                Tutar = odeme.OdemeMiktari,
                BakiyeOnce = alacakliBakiyeOnce,
                BakiyeSonra = odeme.AlacakliHesap.Bakiye,
                Tarih = DateTime.Now,
                IslemTipi = "Alacak",
                Aciklama = odeme.OdemeAciklamasi
            };

            // 4. MUHASEBE DEFTERİ: Borç Kaydı (Sistem Hesabı İçin - Net Bakiye Üzerinden)
            var borcKaydi = new MuhasebeDefteri
            {
                OdemeId = odeme.Id,
                HesapId = odeme.BorcluHesapId.Value,
                Tutar = odeme.OdemeMiktari,
                BakiyeOnce = borcluBakiyeOnce,
                BakiyeSonra = odeme.BorcluHesap.Bakiye,
                Tarih = DateTime.Now,
                IslemTipi = "Borç",
                Aciklama = odeme.OdemeAciklamasi
            };

            _db.MuhasebeDefteri.AddRange(alacakKaydi, borcKaydi);

            _db.SaveChanges();
            transaction.Commit();

            return (true, "Ödeme başarıyla gerçekleşti.");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return (false, $"Hata: {ex.Message}");
        }
    }
}