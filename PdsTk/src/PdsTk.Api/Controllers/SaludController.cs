using Microsoft.AspNetCore.Mvc;

namespace PdsTk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SaludController : ControllerBase
{
    [HttpGet]
    [Route("2")]
    public IActionResult Obtener()
    {
        return Ok(new
        {
            mensaje = "Controlador de salud funcionando xdxddxd",
            fechaUtc = DateTime.UtcNow
        });
    }
}