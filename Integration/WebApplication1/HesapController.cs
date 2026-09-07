using Microsoft.AspNetCore.Mvc;
using DataAccess;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HesapController : ControllerBase
{
    private readonly HesapServisi _hesapServisi;

    public HesapController(HesapServisi hesapServisi)
    {
        _hesapServisi = hesapServisi;
    }

    [HttpGet("bekleyen-odemeler/{hesapId}")]
    public IActionResult BekleyenOdemeler(int hesapId)
    {
        var odemeler = _hesapServisi.BekleyenOdemeleriGetir(hesapId);
        return Ok(odemeler);
    }
}