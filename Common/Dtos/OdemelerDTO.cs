namespace Dtos;

public class OdemeDTO
{
    
    public int Id { get; set; }
    public string HesapNo { get; set; } = string.Empty;
    public decimal Miktar { get; set; }
    public string Aciklama { get; set; } = string.Empty;
    public DateTime SonOdemeTarihi { get; set; }
    public bool OdendiMi { get; set; }
    public DateTime? OdemeTarihi { get; set; }
}