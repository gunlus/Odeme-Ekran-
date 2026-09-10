using Microsoft.AspNetCore.Mvc;
using Business; // DataAccess yerine Business eklendi
using Dtos;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MusteriController : ControllerBase
{
    private readonly IMusteriServisi _musteriServisi; // Somut sınıf yerine Arayüz (Interface) kullanıldı

    public MusteriController(IMusteriServisi musteriServisi)
    {
        _musteriServisi = musteriServisi;
    }

    [HttpGet("{tckn}")]
    public IActionResult GetByTCKN(string tckn)
    {
        var musteri = _musteriServisi.TCKNIleGetir(tckn);
        if (musteri == null)
            return NotFound("Müşteri bulunamadı.");
        return Ok(musteri);
    }
}