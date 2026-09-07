namespace Dtos;

public class HesapDTO
{
    public int Id { get; set; }
    public string HesapNo { get; set; } = string.Empty;
    public decimal Bakiye { get; set; }
    public int MusteriId { get; set; }
}