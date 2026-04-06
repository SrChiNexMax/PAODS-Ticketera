using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PdsTk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class SaludController : ControllerBase
{
    [HttpGet]
    public IActionResult Obtener()
    {
        return Ok(new
        {
            mensaje = "API PdsTk funcionando correctamente",
            fechaUtc = DateTime.UtcNow
        });
    }
}
