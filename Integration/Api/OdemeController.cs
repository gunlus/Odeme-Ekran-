using Microsoft.AspNetCore.Mvc;
using Business; // DataAccess yerine Business eklendi

namespace WebApplication1; // (veya namespace WebApplication1.Controllers; olarak da bırakabilirsin)

[ApiController]
[Route("api/[controller]")]
public class OdemeController : ControllerBase
{
    private readonly IOdemeServisi _odemeServisi;
    
    public OdemeController(IOdemeServisi odemeServisi)
    {
        _odemeServisi = odemeServisi;
    }
    
    [HttpPost("{id}")]
    public IActionResult OdemeYap(int id)
    {
        var sonuc = _odemeServisi.OdemeYap(id);
        
        if (sonuc.basarili)
            return Ok(new { mesaj = sonuc.mesaj });
        else
            return BadRequest(new { hata = sonuc.mesaj });
    }

    [HttpGet("bekleyen/{hesapId}")]
    public IActionResult BekleyenOdemeler(int hesapId)
    {
        return Ok("Şimdilik Boş , yakında çalışacak.");
    }
}