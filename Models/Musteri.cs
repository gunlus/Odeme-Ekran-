using System.Collections.Generic;

namespace Odeme_Projesi.Models;

public class Musteri
{
    public int Id { get; set; }
    public string TCKN { get; set; }
    public string Isim { get; set; }
    public string Soyisim { get; set; }
    
    public ICollection<Hesap> Hesaplar { get; set; } = new List<Hesap>();

    public Musteri() { }
    
    public Musteri(string tckn, string isim, string soyisim)
    {
        TCKN = tckn;
        Isim = isim;
        Soyisim = soyisim;
    }
}