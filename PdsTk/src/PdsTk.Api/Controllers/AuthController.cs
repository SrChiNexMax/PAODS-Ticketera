using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PdsTk.Api.Extensions;
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
        var respuesta = await _iniciarSesionCasoUso.EjecutarAsync(solicitud, cancellationToken);
        return Ok(respuesta);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> PerfilActual(CancellationToken cancellationToken)
    {
        var perfil = await _obtenerPerfilActualCasoUso.EjecutarAsync(User.ObtenerUsuarioId(), cancellationToken);
        return Ok(perfil);
    }
}
