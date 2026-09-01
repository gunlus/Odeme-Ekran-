using System;
using System.IO;
using Odeme_Projesi.Data;
using Odeme_Projesi.Models;
using Microsoft.EntityFrameworkCore;

namespace Odeme_Projesi.Services;

public class LogServisi
{
    private readonly string _logDosyasi = "logs/uygulama.log";

    public LogServisi()
    {
        
        string klasor = Path.GetDirectoryName(_logDosyasi);
        if (!Directory.Exists(klasor))
            Directory.CreateDirectory(klasor);
    }

   
    public void LogYaz(string mesaj, string seviye = "BİLGİ", string kaynak = "")
    {
        // 1. TXT'ye yaz (HATA OLSA BILE SESSIZ GEÇ)
        try
        {
            string logSatiri = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {seviye} | {mesaj}";
            File.AppendAllText(_logDosyasi, logSatiri + Environment.NewLine);
        }
        catch (Exception ex)
        {
            // TXT yazılamazsa bunu konsola yaz ama uygulamayı patlatma!
            Console.WriteLine($"TXT Log Hatası: {ex.Message}");
        }

        // 2. Veritabanına yaz (HATA OLSA BILE SESSIZ GEÇ)
        try
        {
            // ✅ KRİTİK: Log işlemi için YENİ bir DbContext örneği oluştur.
            // Bu, ana işlemin (ödeme) transaction'ına dahil olmaz!
            using (var db = new BankaDbContext())
            {
                var log = new Log
                {
                    Seviye = seviye,
                    Mesaj = mesaj,
                    Tarih = DateTime.Now,
                    Kaynak = kaynak
                };
                db.Loglar.Add(log);
                db.SaveChanges(); // Bu işlem başarısız olursa sadece log kaybolur, ödeme işlemi etkilenmez.
            }
        }
        catch (Exception ex)
        {
            // DB'ye yazılamazsa bunu konsola yaz ama uygulamayı patlatma!
            Console.WriteLine($"DB Log Hatası: {ex.Message}");
        }
    }
    
    public void Bilgi(string mesaj, string kaynak = "") => LogYaz(mesaj, "BİLGİ", kaynak);
    public void Uyari(string mesaj, string kaynak = "") => LogYaz(mesaj, "UYARI", kaynak);
    public void Hata(string mesaj, string kaynak = "") => LogYaz(mesaj, "HATA", kaynak);
}