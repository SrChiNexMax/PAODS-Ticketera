using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PdsTk.Api.Extensions;
using PdsTk.Aplicacion.CasosDeUso.Incidentes;
using PdsTk.Aplicacion.Dtos;
using PdsTk.Dominio.Enumeradores;

namespace PdsTk.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class IncidentesController(
    CrearIncidenteCasoUso crearIncidenteCasoUso,
    ListarIncidentesCasoUso listarIncidentesCasoUso,
    ObtenerDetalleIncidenteCasoUso obtenerDetalleIncidenteCasoUso,
    AsignarIncidenteCasoUso asignarIncidenteCasoUso,
    CambiarEstadoIncidenteCasoUso cambiarEstadoIncidenteCasoUso,
    AgregarComentarioIncidenteCasoUso agregarComentarioIncidenteCasoUso,
    ResolverIncidenteCasoUso resolverIncidenteCasoUso,
    CerrarIncidenteCasoUso cerrarIncidenteCasoUso) : ControllerBase
{
    private readonly CrearIncidenteCasoUso _crearIncidenteCasoUso = crearIncidenteCasoUso;
    private readonly ListarIncidentesCasoUso _listarIncidentesCasoUso = listarIncidentesCasoUso;
    private readonly ObtenerDetalleIncidenteCasoUso _obtenerDetalleIncidenteCasoUso = obtenerDetalleIncidenteCasoUso;
    private readonly AsignarIncidenteCasoUso _asignarIncidenteCasoUso = asignarIncidenteCasoUso;
    private readonly CambiarEstadoIncidenteCasoUso _cambiarEstadoIncidenteCasoUso = cambiarEstadoIncidenteCasoUso;
    private readonly AgregarComentarioIncidenteCasoUso _agregarComentarioIncidenteCasoUso = agregarComentarioIncidenteCasoUso;
    private readonly ResolverIncidenteCasoUso _resolverIncidenteCasoUso = resolverIncidenteCasoUso;
    private readonly CerrarIncidenteCasoUso _cerrarIncidenteCasoUso = cerrarIncidenteCasoUso;

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] EstadoIncidente? estado,
        [FromQuery] PrioridadIncidente? prioridad,
        [FromQuery] int? agenteAsignadoId,
        [FromQuery] int? solicitanteId,
        [FromQuery] bool soloActivos = true,
        CancellationToken cancellationToken = default)
    {
        var resultado = await _listarIncidentesCasoUso.EjecutarAsync(
            new ConsultaIncidentesFiltro(
                User.ObtenerUsuarioId(),
                User.ObtenerRol(),
                estado,
                prioridad,
                agenteAsignadoId,
                solicitanteId,
                soloActivos),
            cancellationToken);

        return Ok(resultado);
    }

    [HttpGet("{incidenteId:int}")]
    public async Task<IActionResult> ObtenerPorId(int incidenteId, CancellationToken cancellationToken)
    {
        var resultado = await _obtenerDetalleIncidenteCasoUso.EjecutarAsync(
            new ObtenerDetalleIncidenteSolicitud(
                incidenteId,
                User.ObtenerUsuarioId(),
                User.ObtenerRol()),
            cancellationToken);

        return Ok(resultado);
    }

    [HttpPost]
    [Authorize(Roles = "Solicitante")]
    public async Task<IActionResult> Crear([FromBody] CrearIncidenteRequest request, CancellationToken cancellationToken)
    {
        var resultado = await _crearIncidenteCasoUso.EjecutarAsync(
            new CrearIncidenteSolicitud(
                User.ObtenerUsuarioId(),
                User.ObtenerRol(),
                request.Titulo,
                request.Descripcion,
                request.Prioridad),
            cancellationToken);

        return CreatedAtAction(nameof(ObtenerPorId), new { incidenteId = resultado.Id }, resultado);
    }

    [HttpPost("{incidenteId:int}/asignaciones")]
    [Authorize(Roles = "Administrador,Supervisor")]
    public async Task<IActionResult> Asignar(int incidenteId, [FromBody] AsignarIncidenteRequest request, CancellationToken cancellationToken)
    {
        var resultado = await _asignarIncidenteCasoUso.EjecutarAsync(
            new AsignarIncidenteSolicitud(
                incidenteId,
                User.ObtenerUsuarioId(),
                User.ObtenerRol(),
                request.AgenteId),
            cancellationToken);

        return Ok(resultado);
    }

    [HttpPatch("{incidenteId:int}/estado")]
    [Authorize(Roles = "Administrador,Supervisor")]
    public async Task<IActionResult> CambiarEstado(int incidenteId, [FromBody] CambiarEstadoIncidenteRequest request, CancellationToken cancellationToken)
    {
        var resultado = await _cambiarEstadoIncidenteCasoUso.EjecutarAsync(
            new CambiarEstadoIncidenteSolicitud(
                incidenteId,
                User.ObtenerUsuarioId(),
                User.ObtenerRol(),
                request.NuevoEstado),
            cancellationToken);

        return Ok(resultado);
    }

    [HttpPost("{incidenteId:int}/comentarios")]
    [Authorize(Roles = "Administrador,Supervisor,Agente,Solicitante")]
    public async Task<IActionResult> AgregarComentario(int incidenteId, [FromBody] AgregarComentarioIncidenteRequest request, CancellationToken cancellationToken)
    {
        var resultado = await _agregarComentarioIncidenteCasoUso.EjecutarAsync(
            new AgregarComentarioIncidenteSolicitud(
                incidenteId,
                User.ObtenerUsuarioId(),
                User.ObtenerRol(),
                request.Contenido,
                request.EsInterno),
            cancellationToken);

        return Ok(resultado);
    }

    [HttpPost("{incidenteId:int}/resolver")]
    [Authorize(Roles = "Administrador,Supervisor,Agente")]
    public async Task<IActionResult> Resolver(int incidenteId, CancellationToken cancellationToken)
    {
        var resultado = await _resolverIncidenteCasoUso.EjecutarAsync(
            new ResolverIncidenteSolicitud(
                incidenteId,
                User.ObtenerUsuarioId(),
                User.ObtenerRol()),
            cancellationToken);

        return Ok(resultado);
    }

    [HttpPost("{incidenteId:int}/cerrar")]
    [Authorize(Roles = "Administrador,Supervisor,Solicitante")]
    public async Task<IActionResult> Cerrar(int incidenteId, CancellationToken cancellationToken)
    {
        var resultado = await _cerrarIncidenteCasoUso.EjecutarAsync(
            new CerrarIncidenteSolicitud(
                incidenteId,
                User.ObtenerUsuarioId(),
                User.ObtenerRol()),
            cancellationToken);

        return Ok(resultado);
    }

    public record CrearIncidenteRequest(string Titulo, string Descripcion, PrioridadIncidente Prioridad);

    public record AsignarIncidenteRequest(int AgenteId);

    public record CambiarEstadoIncidenteRequest(EstadoIncidente NuevoEstado);

    public record AgregarComentarioIncidenteRequest(string Contenido, bool EsInterno);
}
