using Microsoft.AspNetCore.Mvc;
using DataAccess;
using Dtos; // DTO'ların olduğu namespace (Entities projesinde tanımlıysa onu kullan)

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MusteriController : ControllerBase
{
    private readonly MusteriServisi _musteriServisi;

    public MusteriController(MusteriServisi musteriServisi)
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