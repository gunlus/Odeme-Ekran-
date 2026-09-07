namespace Dtos;
public class MusteriDTO
{
    public int Id { get; set; }
    public string TCKN { get; set; }
    public string Isim { get; set; }
    public string Soyisim { get; set; }
    public List<HesapDTO> Hesaplar { get; set; }  // Sadece UI'ın ihtiyacı olanlar
}