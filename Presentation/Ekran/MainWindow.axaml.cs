using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Dtos;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;

namespace Ekran;

public partial class MainWindow : Window
{
    private readonly HttpClient _httpClient;
    private readonly string _apiBaseUrl = "http://localhost:5179"; 

    public MainWindow()
    {
        InitializeComponent();
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(_apiBaseUrl);
    }

    // ==============================
    // EKRANDAKİ ÖDEME YAP BUTONU
    // ==============================
    private async void OdemeYap_Click(object? sender, RoutedEventArgs e)
    {
        if (LstBorclar.SelectedItem is not OdemeDTO seciliOdeme)
        {
            GosterMesaj("Lütfen bir ödeme seçin.");
            return;
        }

        try
        {
            var response = await _httpClient.PostAsync($"api/Odeme/{seciliOdeme.Id}", null);
            
            // API'den dönen JSON'ı okuyup mesajı çıkarıyoruz
            string jsonString = await response.Content.ReadAsStringAsync();
            string mesaj = "İşlem tamamlandı.";

            using (JsonDocument doc = JsonDocument.Parse(jsonString))
            {
                if (doc.RootElement.TryGetProperty("mesaj", out var mesajProp))
                    mesaj = mesajProp.GetString() ?? mesaj;
                else if (doc.RootElement.TryGetProperty("hata", out var hataProp))
                    mesaj = hataProp.GetString() ?? mesaj;
            }

            if (response.IsSuccessStatusCode)
            {
                GosterMesaj($"✅ Başarılı: {mesaj}", true);
                
                
                if (CmbHesaplar.SelectedItem is HesapDTO)
                {
                    await SeciliHesabinOdemeleriniYenile();
                    
                }
            }
            else
            {
                GosterMesaj($"❌ Hata: {mesaj}");
            }
        }
        catch (Exception ex)
        {
            GosterMesaj($"❌ API Hatası: {ex.Message}");
        }
    }

    // ==============================
    // TC KİMLİK NO YAZARKEN KONTROL
    // ==============================
    private void TxtAramaBari_TextChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
    {
        TxtHataMesaji.IsVisible = false;

        if (sender is TextBox textBox && !string.IsNullOrEmpty(textBox.Text))
        {
            string sadeceRakamlar = new string(textBox.Text.Where(char.IsDigit).ToArray());

            if (sadeceRakamlar.StartsWith("0"))
            {
                sadeceRakamlar = sadeceRakamlar.Substring(1);
            }

            if (textBox.Text != sadeceRakamlar)
            {
                textBox.Text = sadeceRakamlar;
                textBox.CaretIndex = sadeceRakamlar.Length;
            }
        }
    }

    // ==============================
    // SORGULA BUTONU
    // ==============================
    private async void Sorgula_Click(object sender, RoutedEventArgs e)
    {
        string input = TxtAramaBari.Text?.Trim() ?? "";

        if (string.IsNullOrEmpty(input))
        {
            GosterMesaj("❌ TC Kimlik Numarası boş bırakılamaz.");
            return;
        }

        if (input.StartsWith("0"))
        {
            GosterMesaj("❌ TC Kimlik Numarası 0 ile başlayamaz.");
            return;
        }

        if (input.Length != 11)
        {
            GosterMesaj("❌ TC Kimlik Numarası tam olarak 11 haneli olmalıdır.");
            return;
        }

        // Temizlik
        CmbHesaplar.ItemsSource = null;
        CmbHesaplar.IsEnabled = false;
        CmbHesaplar.SelectedItem = null;
        LstBorclar.ItemsSource = null;
        PnlListeAlani.IsVisible = false;
        BtnOdemeYap.IsVisible = false;

        try
        {
            var response = await _httpClient.GetAsync($"api/Musteri/{input}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                GosterMesaj("Müşteri bulunamadı.");
                return;
            }

            response.EnsureSuccessStatusCode();

            var musteri = await response.Content.ReadFromJsonAsync<MusteriDTO>();

            if (musteri != null)
            {
                if (musteri.Hesaplar != null && musteri.Hesaplar.Any())
                {
                    CmbHesaplar.ItemsSource = musteri.Hesaplar;
                    CmbHesaplar.IsEnabled = true;
                    // SelectedIndex = 0 yapılınca CmbHesaplar_SelectionChanged otomatik tetiklenir
                    CmbHesaplar.SelectedIndex = 0; 
                }
                else
                {
                    PnlListeAlani.IsVisible = false;
                    GosterMesaj("ℹ️ Müşteriye ait hesap bulunamadı.");
                }
            }
        }
        catch (Exception ex)
        {
            GosterMesaj($"❌ API Hatası: {ex.Message}");
        }
    }

    // ==============================
    // HATA / BİLGİ MESAJINI GÖSTERME (LABEL)
    // ==============================
    private void GosterMesaj(string mesaj, bool basariliMi = false)
    {
        TxtHataMesaji.Text = mesaj;
        TxtHataMesaji.Foreground = basariliMi 
            ? Avalonia.Media.Brush.Parse("#27AE60") 
            : Avalonia.Media.Brush.Parse("#E74C3C");
        
        TxtHataMesaji.IsVisible = true;
    }

    // ==============================
    // TEMİZLE BUTONU
    // ==============================
    private void Temizle_Click(object? sender, RoutedEventArgs e)
    {
        TxtAramaBari.Text = string.Empty;
        LstBorclar.ItemsSource = null;
        PnlListeAlani.IsVisible = false;
        BtnOdemeYap.IsVisible = false;
        CmbHesaplar.ItemsSource = null;
        CmbHesaplar.IsEnabled = false;
        CmbHesaplar.SelectedItem = null;
        
        TxtHataMesaji.IsVisible = false;
        TxtHataMesaji.Text = string.Empty;

        Console.WriteLine("✅ Ekran temizlendi.");
    }

    // ==============================
    // HESAP SEÇİMİ DEĞİŞTİĞİNDE
    // ==============================
    public async void CmbHesaplar_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
        await SeciliHesabinOdemeleriniYenile();
    }

    // ==============================
    // LİSTE SEÇİMİ DEĞİŞTİĞİNDE
    // ==============================
    public void LstBorclar_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
        if (LstBorclar.SelectedItem is OdemeDTO seciliOdeme)
        {
            Console.WriteLine($"Seçilen ödeme ID: {seciliOdeme.Id}");
        }
        else
        {
            Console.WriteLine("Seçili öğe yok veya geçersiz tip.");
        }
    }
    
    private async Task SeciliHesabinOdemeleriniYenile()
    {
        if (CmbHesaplar.SelectedItem is HesapDTO seciliHesap)
        {
            try
            {
                var odemeler = await _httpClient.GetFromJsonAsync<List<OdemeDTO>>($"api/Hesap/bekleyen-odemeler/{seciliHesap.Id}")
                               ?? new List<OdemeDTO>();

                LstBorclar.ItemsSource = odemeler;
                PnlListeAlani.IsVisible = odemeler.Any();
                BtnOdemeYap.IsVisible = odemeler.Any();

                if (odemeler.Any())
                {
                    LstBorclar.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                GosterMesaj($"❌ API Hatası: {ex.Message}");
            }
        }
    }
}