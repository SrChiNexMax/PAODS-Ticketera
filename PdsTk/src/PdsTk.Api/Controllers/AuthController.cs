using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PdsTk.Aplicacion.CasosDeUso.Autenticacion;
using PdsTk.Aplicacion.Dtos;

namespace PdsTk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IniciarSesionCasoUso iniciarSesionCasoUso,
    ObtenerPerfilActualCasoUso obtenerPerfilActualCasoUso) : ControllerBase
{
    private readonly IniciarSesionCasoUso _iniciarSesionCasoUso = iniciarSesionCasoUso;
    private readonly ObtenerPerfilActualCasoUso _obtenerPerfilActualCasoUso = obtenerPerfilActualCasoUso;

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] IniciarSesionSolicitud solicitud, CancellationToken cancellationToken)
    {
        try
        {
            var respuesta = await _iniciarSesionCasoUso.EjecutarAsync(solicitud, cancellationToken);
            return Ok(respuesta);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { mensaje = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> PerfilActual(CancellationToken cancellationToken)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var usuarioId))
        {
            return Unauthorized(new { mensaje = "Token invalido." });
        }

        try
        {
            var perfil = await _obtenerPerfilActualCasoUso.EjecutarAsync(usuarioId, cancellationToken);
            return Ok(perfil);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { mensaje = ex.Message });
        }
    }
}
