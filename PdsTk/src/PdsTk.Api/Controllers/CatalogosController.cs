using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PdsTk.Aplicacion.CasosDeUso.PoliticasSla;
using PdsTk.Aplicacion.CasosDeUso.Usuarios;

namespace PdsTk.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CatalogosController(
    ListarPoliticasSlaCasoUso listarPoliticasSlaCasoUso,
    ListarAgentesCasoUso listarAgentesCasoUso) : ControllerBase
{
    private readonly ListarPoliticasSlaCasoUso _listarPoliticasSlaCasoUso = listarPoliticasSlaCasoUso;
    private readonly ListarAgentesCasoUso _listarAgentesCasoUso = listarAgentesCasoUso;

    [HttpGet("politicas-sla")]
    public async Task<IActionResult> ListarPoliticasSla(CancellationToken cancellationToken)
    {
        var resultado = await _listarPoliticasSlaCasoUso.EjecutarAsync(cancellationToken);
        return Ok(resultado);
    }

    [HttpGet("agentes")]
    [Authorize(Roles = "Administrador,Supervisor")]
    public async Task<IActionResult> ListarAgentes(CancellationToken cancellationToken)
    {
        var resultado = await _listarAgentesCasoUso.EjecutarAsync(cancellationToken);
        return Ok(resultado);
    }
}
