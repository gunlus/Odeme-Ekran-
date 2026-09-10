using System;
namespace DataAccess;

using Microsoft.EntityFrameworkCore;
using Dtos;
using Entities;

public class MusteriRepository : IMusteriRepository
{
    private readonly BankaDbContext _db;

    public MusteriRepository(BankaDbContext db)
    {
        _db = db;
    }

    public MusteriDTO? TCKNileGetir(string tckn)
    {
        return _db.Musteriler
            .Include(m => m.Hesaplar)
            .Where(m => m.TCKN == tckn)
            .Select(m => new MusteriDTO
            {
                Id = m.Id,
                TCKN = m.TCKN,
                Isim = m.Isim,
                Soyisim = m.Soyisim,
                Hesaplar = m.Hesaplar.Select(h => new HesapDTO
                {
                    Id = h.Id,
                    HesapNo = h.HesapNo,
                    Bakiye = h.Bakiye,
                    MusteriId = h.MusteriId
                }).ToList()
            })
            .FirstOrDefault();
    }

    public List<MusteriDTO> TumMusterileriGetir()
    {
        return _db.Musteriler
            .Include(m => m.Hesaplar)
            .Select(m => new MusteriDTO
            {
                Id = m.Id,
                TCKN = m.TCKN,
                Isim = m.Isim,
                Soyisim = m.Soyisim,
                Hesaplar = m.Hesaplar.Select(h => new HesapDTO
                {
                    Id = h.Id,
                    HesapNo = h.HesapNo,
                    Bakiye = h.Bakiye,
                    MusteriId = h.MusteriId
                }).ToList()
            })
            .ToList();
    }

    public void MusteriEkle(MusteriDTO musteriDTO)
    {
        var musteri = new Musteri
        {
            TCKN = musteriDTO.TCKN,
            Isim = musteriDTO.Isim,
            Soyisim = musteriDTO.Soyisim
        };
        _db.Musteriler.Add(musteri);
        _db.SaveChanges();
    }

    public bool MusteriVarMi()
    {
        return _db.Musteriler.Any();
    }
}