using Entities;
using Microsoft.EntityFrameworkCore;
using System.IO;
namespace DataAccess;


public static class DatabaseInitializer
{
    public static void Initialize()
    {
        using var db = new BankaDbContext();
        db.Database.EnsureCreated();
    }
    public static void TestVerileriEkle()
{
    Random rastgele = new Random();
    
    // ========== 1. ÖDEME AÇIKLAMALARI ==========
    string[] odemeAciklamalari = new string[20]
    {
        "Ağustos 2026 Dönemi Net Maaş Ödemesi",
        "Temmuz Dönemi Performans ve Prim Hak Edişi",
        "Hafta Sonu Fazla Çalışma (Ek Mesai) Ücreti",
        "Resmi Tatil Günleri Ek Mesai Hak Ediş Tahsilatı",
        "Ağustos Dönemi Yemek Yardımı Nakdi Ödemesi",
        "Yıllık İzin Süreklilik ve Yol Yardımı Ödeneği",
        "Çocuk ve Aile Geçim Yardımı Tazminatı",
        "Gece Vardiyası Ek Çalışma Tazminat Bedeli",
        "Kıdem Tazminatı Hak Ediş Ödemesi",
        "İhbar Tazminatı ve Ayrılış Paketi Ödemesi",
        "Yıllık İkramiye (1. Taksit) Hak Ediş Tahsilatı",
        "Yol ve Ulaşım Ödeneği Aylık Nakdi Ödeme",
        "Kasa Tazminatı ve Mali Sorumluluk Ödeneği",
        "Bayram Yardımı ve Sosyal Destek Ödeneği",
        "Eğitim ve Öğretim Yılı Başlangıç Yardımı Gideri",
        "Evlilik ve Aile Kurma Sosyal Yardımı Ödemesi",
        "Doğum Yardımı ve Özlük Hakları Ödeneği",
        "Giyim ve Koruyucu Ekipman Tedarik Yardımı",
        "Projeye Dayalı Başarı Ödülü ve Prim Ödemesi",
        "Saha / Arazi Görev Yolluğu (Harcırah) Ödemesi"
    };

    string[] isimler = new string[50]
    {
        "Alperen", "Barış", "Can", "Deniz", "Emre", "Furkan", "Gökhan", "Hakan", "Kaan", "Kerem",
        "Mert", "Oğuz", "Ömer", "Burak", "Yiğit", "Aslı", "Beren", "Ceren", "Defne", "Elif",
        "Eylül", "Gamze", "Hazal", "Irmak", "İrem", "Melis", "Merve", "Selin", "Zeynep", "Yağmur",
        "Ahmet", "Mehmet", "Ali", "Mustafa", "Murat", "Onur", "Volkan", "Serkan", "Tolga", "Arda",
        "Bora", "Ege", "Utku", "Gaye", "Seda", "Buse", "Gizem", "Dilan", "Demet", "Pınar"
    };

    string[] soyIsimler = new string[50]
    {
        "Yılmaz", "Kaya", "Demir", "Çelik", "Şahin", "Yıldız", "Yıldırım", "Öztürk", "Aydın", "Özdemir",
        "Arslan", "Doğan", "Kılıç", "Aslan", "Çetin", "Kara", "Koç", "Kurt", "Avcı", "Sarı",
        "Yalçın", "Köse", "Ateş", "Polat", "Erdoğan", "Bulut", "Korkmaz", "Güneş", "Keser", "Yavuz",
        "Şen", "Acar", "Aksoy", "Uzun", "Özkan", "Güler", "Köseoglu", "Aktaş", "Uçar", "Tekin",
        "Çakır", "Erten", "Gök", "Bakır", "Kartal", "Tuncer", "Yiğit", "Gündüz", "Şimşek", "Pala"
    };

    var musteriler = new List<Musteri>();
    var hesaplar = new List<Hesap>();
    var odemeler = new List<Odeme>();

    long tcknBaslangic = 12345678901;
    int musteriSayisi = 100;
    int odemeSayisi = 200; // ✅ 200 ödeme

    // ========== 4. MÜŞTERİLERİ OLUŞTUR ==========
    for (int i = 0; i < musteriSayisi; i++)
    {
        int isimIndex = rastgele.Next(isimler.Length);
        int soyIndex = rastgele.Next(soyIsimler.Length);

        string tckn = (tcknBaslangic + i).ToString();
        string isim = isimler[isimIndex];
        string soyisim = soyIsimler[soyIndex];

        var musteri = new Musteri(tckn, isim, soyisim);
        musteriler.Add(musteri);
    }

    using var db = new BankaDbContext();

    db.Musteriler.AddRange(musteriler);
    db.SaveChanges();

    // ========== 6. HESAPLARI OLUŞTUR (Her müşteriye 1-3 hesap) ==========
    foreach (var musteri in musteriler)
    {
        int hesapSayisi = rastgele.Next(1, 4);
        for (int j = 0; j < hesapSayisi; j++)
        {
            string hesapNo = $"TR{DateTime.Now:yyyyMMdd}{musteri.Id}{j+1:D2}";
            int ka = rastgele.Next(1000, 3000);
            int kb = rastgele.Next(0, 800);
            var hesap = new Hesap(hesapNo, musteri.Id, ka, kb);
            hesaplar.Add(hesap);
        }
    }

    db.Hesaplar.AddRange(hesaplar);
    db.SaveChanges();

    // ========== 7. ÖDEMELERİ OLUŞTUR ==========
    // ✅ Ödenmiş ödeme sayısını maksimum 6 ile sınırla
    int odenecekSayisi = 0;

    for (int i = 0; i < odemeSayisi; i++)
    {
        var hesap = hesaplar[rastgele.Next(hesaplar.Count)];
        int odemeMiktar = rastgele.Next(100, 50000);
        int aciklamaIndex = rastgele.Next(odemeAciklamalari.Length);
        
        // ✅ Ödenmiş olma durumunu belirle (maksimum 6)
        bool odemeDurumu;
        if (odenecekSayisi < 6 && rastgele.Next(0, 3) == 0) // %33 ihtimal
        {
            odemeDurumu = true;
            odenecekSayisi++;
        }
        else
        {
            odemeDurumu = false;
        }

        var odeme = new Odeme
        {
            AlacakliHesapId = hesap.Id,
            BorcluHesapId = null,
            OdemeMiktari = odemeMiktar,
            OdemeAciklamasi = odemeAciklamalari[aciklamaIndex],
            OdemeDurumu = odemeDurumu,
            SonOdemeTarihi = DateTime.Now.AddDays(rastgele.Next(1, 90))
        };

        // Eğer ödenmişse bakiyeyi güncelle
        if (odemeDurumu)
        {
            hesap.KumuleAlacakArttir(odemeMiktar);
        }

        odemeler.Add(odeme);
    }

    db.Odemeler.AddRange(odemeler);
    db.SaveChanges();

    Console.WriteLine($"✅ {musteriSayisi} müşteri, {hesaplar.Count} hesap ve {odemeler.Count} ödeme eklendi!");
    Console.WriteLine($"   - Ödenen: {odemeler.Count(o => o.OdemeDurumu == true)}");
    Console.WriteLine($"   - Bekleyen: {odemeler.Count(o => o.OdemeDurumu == false)}");
}
}