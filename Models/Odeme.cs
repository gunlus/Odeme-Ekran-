using System;

namespace Odeme_Projesi.Models;

public class Odeme
{
    public int Id { get; set; }

    // ✅ Yeni: Hesap bazlı Foreign Key'ler
    public int AlacakliHesapId { get; set; }
    public int? BorcluHesapId { get; set; }

    // ✅ Yeni: Navigation Property'ler (Hesap ile)
    public Hesap AlacakliHesap { get; set; }
    public Hesap BorcluHesap { get; set; }

    public decimal OdemeMiktari { get; set; }
    public string OdemeAciklamasi { get; set; }
    public DateTime SonOdemeTarihi { get; set; }
    public bool OdemeDurumu { get; set; } = false;

    public Odeme() { }

    public Odeme(int alacakliHesapId, int? borcluHesapId, decimal odemeMiktari,string odemeAciklamasi, DateTime sonOdemeTarihi)
    {
        AlacakliHesapId = alacakliHesapId;
        BorcluHesapId = borcluHesapId;
        OdemeMiktari = odemeMiktari;
        OdemeAciklamasi = odemeAciklamasi;
        SonOdemeTarihi = sonOdemeTarihi;
        OdemeDurumu = false;
    }
}