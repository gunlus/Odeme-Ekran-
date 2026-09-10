namespace DataAccess;

using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

public static class DbInitializer
{
    public static void Initialize(BankaDbContext db)
    {
        db.Database.EnsureCreated();

        if (!db.Musteriler.Any())
        {
            TestVerileriEkle(db);
        }
    }

    private static void TestVerileriEkle(BankaDbContext db)
    {
        Random rastgele = new Random();
        
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

        long tcknBaslangic = 12345678901;
        int musteriSayisi = 100;
        int odemeSayisi = 200;
        
        // 1. Önce Sistem Müşterisi ve Banka Hesabı Ekleniyor
        var sistem = new Musteri("10000000000", "Sistem", "Banka");
        db.Musteriler.Add(sistem);
        db.SaveChanges();
        
        var sistemHesabi = new Hesap("BANKA0001", sistem.Id, int.MaxValue, 0);
        db.Hesaplar.Add(sistemHesabi);
        db.SaveChanges();

        // 2. Normal Müşteriler Ekleniyor
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

        db.Musteriler.AddRange(musteriler);
        db.SaveChanges();

        // 3. Hesaplar Ekleniyor
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

        // 4. Ödemeler Oluşturulup En Son Veritabanına Ekleniyor
        var odemeler = new List<Odeme>();
        int odenecekSayisi = 0;

        for (int i = 0; i < odemeSayisi; i++)
        {
            var hesap = hesaplar[rastgele.Next(hesaplar.Count)];
            int odemeMiktar = rastgele.Next(100, 50000);
            int aciklamaIndex = rastgele.Next(odemeAciklamalari.Length);
            
            bool odemeDurumu;
            if (odenecekSayisi < 6 && rastgele.Next(0, 3) == 0)
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
                BorcluHesapId = sistemHesabi.Id,
                OdemeMiktari = odemeMiktar,
                OdemeAciklamasi = odemeAciklamalari[aciklamaIndex],
                OdemeDurumu = odemeDurumu,
                SonOdemeTarihi = DateTime.Now.AddDays(rastgele.Next(1, 90))
            };

            if (odemeDurumu)
            {
                hesap.KumuleAlacakArttir(odemeMiktar);
            }

            odemeler.Add(odeme);
        }

        db.Odemeler.AddRange(odemeler);
        db.SaveChanges();

        Console.WriteLine($"✅ {musteriSayisi} müşteri, {hesaplar.Count} hesap ve {odemeler.Count} ödeme eklendi!");
    }
}