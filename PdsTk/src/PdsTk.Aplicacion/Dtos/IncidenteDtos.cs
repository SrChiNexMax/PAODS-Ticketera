using PdsTk.Dominio.Enumeradores;

namespace PdsTk.Aplicacion.Dtos;

public record ConsultaIncidentesFiltro(
    int UsuarioActualId,
    string RolActual,
    EstadoIncidente? Estado = null,
    PrioridadIncidente? Prioridad = null,
    int? AgenteAsignadoId = null,
    int? SolicitanteId = null,
    bool SoloActivos = true);

public record CrearIncidenteSolicitud(
    int UsuarioActualId,
    string RolActual,
    string Titulo,
    string Descripcion,
    PrioridadIncidente Prioridad);

public record ObtenerDetalleIncidenteSolicitud(int IncidenteId, int UsuarioActualId, string RolActual);

public record AsignarIncidenteSolicitud(int IncidenteId, int UsuarioActualId, string RolActual, int AgenteId);

public record CambiarEstadoIncidenteSolicitud(int IncidenteId, int UsuarioActualId, string RolActual, EstadoIncidente NuevoEstado);

public record AgregarComentarioIncidenteSolicitud(
    int IncidenteId,
    int UsuarioActualId,
    string RolActual,
    string Contenido,
    bool EsInterno);

public record ResolverIncidenteSolicitud(int IncidenteId, int UsuarioActualId, string RolActual);

public record CerrarIncidenteSolicitud(int IncidenteId, int UsuarioActualId, string RolActual);

public record ComentarioIncidenteDto(
    int Id,
    string Contenido,
    bool EsInterno,
    int UsuarioId,
    string UsuarioNombre,
    DateTime FechaCreacion);

public record IncidenteResumenDto(
    int Id,
    string Codigo,
    string Titulo,
    string Estado,
    string Prioridad,
    int SolicitanteId,
    string SolicitanteNombre,
    int? AgenteAsignadoId,
    string? AgenteAsignadoNombre,
    string PoliticaSlaNombre,
    DateTime FechaCreacion,
    DateTime FechaLimitePrimeraRespuesta,
    DateTime FechaLimiteResolucion,
    DateTime? FechaPrimeraRespuesta,
    DateTime? FechaResolucion,
    DateTime? FechaCierre,
    bool PrimeraRespuestaVencida,
    bool ResolucionVencida);

public record IncidenteDetalleDto(
    int Id,
    string Codigo,
    string Titulo,
    string Descripcion,
    string Estado,
    string Prioridad,
    int SolicitanteId,
    string SolicitanteNombre,
    int? AgenteAsignadoId,
    string? AgenteAsignadoNombre,
    PoliticaSlaDto PoliticaSla,
    bool Activo,
    DateTime FechaCreacion,
    DateTime? FechaActualizacion,
    DateTime? FechaPrimeraRespuesta,
    DateTime FechaLimitePrimeraRespuesta,
    DateTime FechaLimiteResolucion,
    DateTime? FechaResolucion,
    DateTime? FechaCierre,
    bool PrimeraRespuestaVencida,
    bool ResolucionVencida,
    bool PuedeAgregarComentarioPublico,
    bool PuedeAgregarComentarioInterno,
    IReadOnlyCollection<ComentarioIncidenteDto> Comentarios);
