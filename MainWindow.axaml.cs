using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Linq;
using Odeme_Projesi.Data; 
using Odeme_Projesi.Models;
using Microsoft.Data.Sqlite;
using System.Data.Common;
using System.Data;
using Odeme_Projesi.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore; // Include için gerekli

namespace Odeme_Projesi;

public partial class MainWindow : Window
{   
    private readonly BankaDbContext _db;
    private readonly OdemeServisi _odemeServisi;
    private readonly LogServisi _logServisi;

    
    public MainWindow()
    {
        try
        {
            InitializeComponent();
            _db = new BankaDbContext();
            _odemeServisi = new OdemeServisi(_db);
            _logServisi = new LogServisi();
            
            _db.Database.EnsureCreated();
            _logServisi.Bilgi("Veritabanı kontrol edildi (EnsureCreated).", "MainWindow");
            
            if (!_db.Musteriler.Any())
            {   
                _logServisi.Bilgi("Veritabanında müşteri yok, veriler ekleniyor...", "MainWindow");
                TestVerileriEkle();
                _logServisi.Bilgi("Test verileri eklendi.", "MainWindow");
            }
        }
        
        catch (Exception ex)
        {
            _logServisi?.Hata($"Veritabanı bağlantı hatası: {ex.Message}", "MainWindow"); 
            GosterMesaj($"Veritabanı bağlantı hatası: {ex.Message}");
        }
        _logServisi.Bilgi("Uygulama başlatma tamamlandı.", "MainWindow");
    }

    public void TestVerileriEkle()
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
        int odemeSayisi = 50;

        // ========== 4. MÜŞTERİLERİ OLUŞTUR (HesapNo kalktı) ==========
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

        _db.Musteriler.AddRange(musteriler);
        _db.SaveChanges();

        // ========== 6. HESAPLARI OLUŞTUR (Her müşteriye 1-3 hesap) ==========
        foreach (var musteri in musteriler)
        {
            int hesapSayisi = rastgele.Next(1, 4);
            for (int j = 0; j < hesapSayisi; j++)
            {
                string hesapNo = $"TR{DateTime.Now:yyyyMMdd}{musteri.Id}{j+1:D2}";
                int ka = rastgele.Next(1000, 3000);
                int kb = rastgele.Next(0,800);
                var hesap = new Hesap(hesapNo, musteri.Id,ka,kb );
                hesaplar.Add(hesap);
            }
        }

        _db.Hesaplar.AddRange(hesaplar);
        _db.SaveChanges();

        // ========== 7. ÖDEMELERİ OLUŞTUR (Hesap ID ile) ==========
        for (int i = 0; i < odemeSayisi; i++)
        {
            var hesap = hesaplar[rastgele.Next(hesaplar.Count)];
            int odemeMiktar = rastgele.Next(100, 50000);
            int aciklamaIndex = rastgele.Next(odemeAciklamalari.Length);
            bool odemeDurumu = rastgele.Next(0, 3) == 0;

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

        _db.Odemeler.AddRange(odemeler);
        _db.SaveChanges();

        Console.WriteLine($"✅ {musteriSayisi} müşteri, {hesaplar.Count} hesap ve {odemeler.Count} ödeme eklendi!");
        Console.WriteLine($"   - Ödenen: {odemeler.Count(o => o.OdemeDurumu == true)}");
        Console.WriteLine($"   - Bekleyen: {odemeler.Count(o => o.OdemeDurumu == false)}");
    }

    private async void Sorgula_Click(object sender, RoutedEventArgs e)
    {
        string input = TxtAramaBari.Text?.Trim() ?? "";

        if (string.IsNullOrEmpty(input))
        {
            _logServisi.Uyari($"Müşteri bulunamadı(Boş T.C Kimlik Numarası girildi): TCKN={input}", "Sorgula_Click");   
            LstBorclar.ItemsSource = null;
            BtnOdemeYap.IsVisible = false;
            await GosterMesaj("TC Kimlik Numarası boş bırakılamaz. Lütfen 11 haneli sayı giriniz.");
            return;
        }
        
        if (input.Length != 11)
        {
            _logServisi.Uyari($"Müşteri bulunamadı(Girilen T.C Kimlik Numarası 11 haneye sahip değil): TCKN={input}", "Sorgula_Click");
            LstBorclar.ItemsSource = null;
            BtnOdemeYap.IsVisible = false;
            await GosterMesaj("TC Kimlik Numarası 11 haneli olmalıdır. Lütfen kontrol ediniz.");
            return;
        }
        
        if (!long.TryParse(input, out _))
        {
            _logServisi.Uyari($"Müşteri bulunamadı TCKN={input}", "Sorgula_Click");
            await GosterMesaj("Bu TC Kimlik Numarasına ait kayıt bulunamadı.");
            LstBorclar.ItemsSource = null;
            BtnOdemeYap.IsVisible = false;
            return;
        }

        // Müşteriyi ve hesaplarını getir
       var musteri = _db.Musteriler
        .Include(m => m.Hesaplar)
        .FirstOrDefault(m => m.TCKN == input);

        if (musteri != null)
        {
            // ComboBox'u doldur
                CmbHesaplar.ItemsSource = musteri.Hesaplar.ToList();
        CmbHesaplar.IsEnabled = true;

        // Varsayılan olarak ilk hesabı seç
        if (musteri.Hesaplar.Any())
        {
            CmbHesaplar.SelectedIndex = 0;
            HesapSecildi(musteri.Hesaplar.First().Id);
            }
        }
        else
        {
            // Müşteri bulunamadı
            CmbHesaplar.ItemsSource = null;
            CmbHesaplar.IsEnabled = false;
            // ...
        }
    }

    private async void OdemeYap_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var secili = LstBorclar.SelectedItem as Odeme;
        if (secili == null)
        {
            _logServisi.Uyari("Ödeme seçilmeden butona tıklandı.", "OdemeYap_Click");
            await GosterMesaj("Lütfen ödemek için bir satır seçin.");
            return;
        }

        var sonuc = _odemeServisi.OdemeYap(secili.Id);
        await GosterMesaj(sonuc.mesaj);

        if (sonuc.basarili)
        {
            _logServisi.Bilgi($"Ödeme başarılı: ID={secili.Id}, Tutar={secili.OdemeMiktari:C}", "OdemeYap_Click");
            // Listeyi yenilemek için mevcut TCKN'yi tekrar sorgula
            Sorgula_Click(sender, e);
        }
        else
        {
            _logServisi.Hata($"Ödeme başarısız: ID={secili.Id}, Hata={sonuc.mesaj}", "OdemeYap_Click");
        }
    }

    private async Task GosterMesaj(string mesaj)
    {
        var msgBox = new Window
        {
            Title = "Bilgi",
            Width = 350,
            Height = 150,
            Content = new StackPanel
            {
                Children =
                {
                    new TextBlock 
                    { 
                        Text = mesaj, 
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap, 
                        Margin = new Avalonia.Thickness(10) 
                    },
                    new Button 
                    { 
                        Content = "Tamam", 
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center, 
                        Margin = new Avalonia.Thickness(10) 
                    }
                }
            }
        };

        ((Button)((StackPanel)msgBox.Content).Children[1]).Click += (s, args) => msgBox.Close();
        await msgBox.ShowDialog(this);
    }

    private void Temizle_Click(object? sender, RoutedEventArgs e)
    {
        TxtAramaBari.Text = string.Empty;
        LstBorclar.ItemsSource = null;
        BtnOdemeYap.IsVisible = false;
        // ComboBox'ı sıfırla
        CmbHesaplar.ItemsSource = null;
        CmbHesaplar.IsEnabled = false;
        // Seçili öğeyi temizle (SelectedItem'ı null yap)
        CmbHesaplar.SelectedItem = null;
        _logServisi.Bilgi("Ekran temizlendi (hesap listesi dahil).", "Temizle_Click");
    }


    private void HesapSecildi(int hesapId)
    {
        var odemeler = _db.Odemeler
            .Include(o => o.AlacakliHesap)
            .Where(o => o.AlacakliHesapId == hesapId && o.OdemeDurumu == false)
            .ToList();

        LstBorclar.ItemsSource = null;
        LstBorclar.ItemsSource = odemeler;
        BtnOdemeYap.IsVisible = odemeler.Any();
    }
    private void CmbHesaplar_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (CmbHesaplar.SelectedItem is Hesap seciliHesap)
        {
            HesapSecildi(seciliHesap.Id);
        }
    }



}