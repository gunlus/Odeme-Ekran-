using System;

namespace Odeme_Projesi.Models;

public class MuhasebeDefteri
{
    public int Id { get; set; }
    public int OdemeId { get; set; }
    public Odeme Odeme { get; set; } = null!;
    public int HesapId { get; set; }
    public Hesap Hesap { get; set; } = null!;
    public decimal Tutar { get; set; }
    public decimal BakiyeOnce { get; set; }
    public decimal BakiyeSonra { get; set; }
    public DateTime Tarih { get; set; }
    public string IslemTipi { get; set; } = string.Empty;
    public string Aciklama { get; set; } = string.Empty;
}