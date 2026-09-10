using Dtos;
namespace DataAccess;

using Microsoft.EntityFrameworkCore;
using Dtos;
using Entities;

public class HesapRepository : IHesapRepository
{
    private readonly BankaDbContext _db;

    public HesapRepository(BankaDbContext db)
    {
        _db = db;
    }

    public HesapDTO? GetirDtoile(string hesapNo)
    {
        return _db.Hesaplar
            .Where(h => h.HesapNo == hesapNo)
            .Select(h => new HesapDTO
            {
                Id = h.Id,
                HesapNo = h.HesapNo,
                Bakiye = h.Bakiye,
                MusteriId = h.MusteriId
            })
            .FirstOrDefault();
    }

    public List<HesapDTO> ListeleDtoile(int musteriId)
    {
        return _db.Hesaplar
            .Where(h => h.MusteriId == musteriId)
            .Select(h => new HesapDTO
            {
                Id = h.Id,
                HesapNo = h.HesapNo,
                Bakiye = h.Bakiye,
                MusteriId = h.MusteriId
            })
            .ToList();
    }

    public void Ekle(HesapDTO hesapDto)
    {
        var yeniHesap = new Hesap(hesapDto.HesapNo, hesapDto.MusteriId, 0, 0);
        _db.Hesaplar.Add(yeniHesap);
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
                OdemeTarihi = null
            })
            .ToList();
    }
}