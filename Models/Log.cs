using System;

namespace Odeme_Projesi.Models;

public class Log
{
    public int Id { get; set; }
    public string Seviye { get; set; } = string.Empty;
    public string Mesaj { get; set; } = string.Empty;
    public DateTime Tarih { get; set; }
    public string? Kaynak { get; set; }
    public Log() { }
    
   
}