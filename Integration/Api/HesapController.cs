using Microsoft.AspNetCore.Mvc;
using Business; // DataAccess yerine Business eklendi

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HesapController : ControllerBase
{
    private readonly IHesapServisi _hesapServisi; // Somut sınıf yerine Arayüz (Interface) kullanıldı

    public HesapController(IHesapServisi hesapServisi)
    {
        _hesapServisi = hesapServisi;
    }

    [HttpGet("bekleyen-odemeler/{hesapId}")]
    public IActionResult BekleyenOdemeler(int hesapId)
    {
        var odemeler = _hesapServisi.BekleyenOdemeleriListele(hesapId);
        return Ok(odemeler);
    }
}