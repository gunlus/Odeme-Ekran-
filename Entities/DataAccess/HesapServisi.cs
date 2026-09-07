using Microsoft.EntityFrameworkCore;
using Entities;
using Dtos;
namespace DataAccess;

public class HesapServisi
{
    private readonly BankaDbContext _db;

    public HesapServisi(BankaDbContext db)
    {
        _db = db;
    }

    public Hesap? HesapGetir(string hesapNo)
    {
        return _db.Hesaplar.Find(hesapNo);
    }

    public List<Hesap> MusteriHesaplari(int musteriId)
    {
        return _db.Hesaplar
            .Where(h => h.MusteriId == musteriId)
            .ToList();
    }

    public void HesapEkle(Hesap hesap)
    {
        _db.Hesaplar.Add(hesap);
        _db.SaveChanges();
    }
    
    public List<OdemeDTO> BekleyenOdemeleriGetir(int hesapId)
    {
        return _db.Odemeler
            .Include(o => o.AlacakliHesap)
            .Where(o => o.AlacakliHesapId == hesapId && o.OdemeDurumu == false)
            .Select(o => new OdemeDTO
            {
                Id = o.Id,
                HesapNo = o.AlacakliHesap.HesapNo,
                Miktar = o.OdemeMiktari,
                Aciklama = o.OdemeAciklamasi,
                SonOdemeTarihi = o.SonOdemeTarihi,
                OdendiMi = o.OdemeDurumu,
                OdemeTarihi = o.OdemeDurumu ? o.SonOdemeTarihi : null
            }).ToList();
    }
}