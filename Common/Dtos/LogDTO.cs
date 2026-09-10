namespace Dtos;

public class LogDTO
{
    public int Id { get; set; }
    public string Mesaj { get; set; } = string.Empty;
    public string Seviye { get; set; } = string.Empty;
    public DateTime Tarih { get; set; }
}