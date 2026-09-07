using Entities;
using Microsoft.EntityFrameworkCore;
using Dtos;  // DTO projesi

namespace DataAccess;

public class MusteriServisi
{
    private readonly BankaDbContext _db;

    public MusteriServisi(BankaDbContext db)
    {
        _db = db;
    }

    // ✅ TCKN ile müşteri getir (DTO döner)
    public MusteriDTO? TCKNIleGetir(string tckn)
    {
        var musteri = _db.Musteriler
            .Include(m => m.Hesaplar)
            .FirstOrDefault(m => m.TCKN == tckn);

        if (musteri == null) return null;

        return new MusteriDTO
        {
            Id = musteri.Id,
            TCKN = musteri.TCKN,
            Isim = musteri.Isim,
            Soyisim = musteri.Soyisim,
            Hesaplar = musteri.Hesaplar.Select(h => new HesapDTO
            {
                Id = h.Id,
                HesapNo = h.HesapNo,
                Bakiye = h.Bakiye,
                MusteriId = h.MusteriId
            }).ToList()
        };
    }

    // ✅ Tüm müşterileri getir (DTO listesi döner)
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
            }).ToList();
    }

    // ✅ Müşteri ekle (Entity alır, DTO döndürmez)
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

    // ✅ Müşteri var mı? (bool döner)
    public bool MusteriVarMi()
    {
        return _db.Musteriler.Any();
    }
}