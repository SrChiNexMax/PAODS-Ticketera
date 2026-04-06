using PdsTk.Aplicacion.Abstracciones.Persistencia;
using PdsTk.Aplicacion.Dtos;
using PdsTk.Aplicacion.Servicios;
using PdsTk.Dominio.Constantes;
using PdsTk.Dominio.Entidades;
using PdsTk.Dominio.Enumeradores;

namespace PdsTk.Aplicacion.CasosDeUso.Incidentes;

public class CrearIncidenteCasoUso(
    IIncidenteRepository incidenteRepository,
    IUsuarioRepository usuarioRepository,
    IRegistroAuditoriaRepository registroAuditoriaRepository,
    IUnitOfWork unitOfWork,
    ISlaService slaService,
    IAuditoriaService auditoriaService,
    ICodigoIncidenteService codigoIncidenteService)
{
    private readonly IIncidenteRepository _incidenteRepository = incidenteRepository;
    private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;
    private readonly IRegistroAuditoriaRepository _registroAuditoriaRepository = registroAuditoriaRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ISlaService _slaService = slaService;
    private readonly IAuditoriaService _auditoriaService = auditoriaService;
    private readonly ICodigoIncidenteService _codigoIncidenteService = codigoIncidenteService;

    public async Task<IncidenteDetalleDto> EjecutarAsync(CrearIncidenteSolicitud solicitud, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(solicitud.Titulo) || string.IsNullOrWhiteSpace(solicitud.Descripcion))
        {
            throw new InvalidOperationException("El titulo y la descripcion son obligatorios.");
        }

        var solicitante = await _usuarioRepository.ObtenerPorIdAsync(solicitud.UsuarioActualId, cancellationToken)
            ?? throw new UnauthorizedAccessException("El usuario autenticado no existe.");

        IncidenteCasoUsoHelper.ValidarUsuarioActivo(solicitante);

        var politicaSla = await _slaService.ObtenerPoliticaActivaPorPrioridadAsync(solicitud.Prioridad, cancellationToken);
        var fechaCreacion = DateTime.UtcNow;
        var limites = _slaService.CalcularLimites(fechaCreacion, politicaSla);

        var incidente = new Incidente
        {
            Codigo = _codigoIncidenteService.GenerarCodigo(),
            Titulo = solicitud.Titulo.Trim(),
            Descripcion = solicitud.Descripcion.Trim(),
            Estado = EstadoIncidente.Nuevo,
            Prioridad = solicitud.Prioridad,
            SolicitanteId = solicitante.Id,
            PoliticaSLAId = politicaSla.Id,
            Activo = true,
            FechaCreacion = fechaCreacion,
            FechaLimitePrimeraRespuesta = limites.fechaLimitePrimeraRespuesta,
            FechaLimiteResolucion = limites.fechaLimiteResolucion,
            PoliticaSLA = politicaSla,
            Solicitante = solicitante
        };

        await _incidenteRepository.AgregarAsync(incidente, cancellationToken);
        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        await _registroAuditoriaRepository.AgregarAsync(
            _auditoriaService.CrearRegistro(
                incidente.Id,
                solicitud.UsuarioActualId,
                TipoAccionAuditoria.Creacion,
                "Incidente creado."),
            cancellationToken);
        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        return await IncidenteCasoUsoHelper.ObtenerDetalleAutorizadoAsync(
            _incidenteRepository,
            incidente.Id,
            solicitud.UsuarioActualId,
            solicitud.RolActual,
            cancellationToken);
    }
}

public class ListarIncidentesCasoUso(IIncidenteRepository incidenteRepository)
{
    private readonly IIncidenteRepository _incidenteRepository = incidenteRepository;

    public async Task<IReadOnlyCollection<IncidenteResumenDto>> EjecutarAsync(ConsultaIncidentesFiltro solicitud, CancellationToken cancellationToken = default)
    {
        var filtro = solicitud;

        if (IncidenteCasoUsoHelper.EsSolicitante(solicitud.RolActual))
        {
            filtro = filtro with
            {
                SolicitanteId = solicitud.UsuarioActualId,
                AgenteAsignadoId = null
            };
        }

        var incidentes = await _incidenteRepository.ListarAsync(filtro, cancellationToken);

        return incidentes
            .Where(x => IncidenteCasoUsoHelper.PuedeVerIncidente(x, solicitud.UsuarioActualId, solicitud.RolActual))
            .Select(IncidenteCasoUsoHelper.MapearResumen)
            .ToArray();
    }
}

public class ObtenerDetalleIncidenteCasoUso(IIncidenteRepository incidenteRepository)
{
    private readonly IIncidenteRepository _incidenteRepository = incidenteRepository;

    public Task<IncidenteDetalleDto> EjecutarAsync(ObtenerDetalleIncidenteSolicitud solicitud, CancellationToken cancellationToken = default)
    {
        return IncidenteCasoUsoHelper.ObtenerDetalleAutorizadoAsync(
            _incidenteRepository,
            solicitud.IncidenteId,
            solicitud.UsuarioActualId,
            solicitud.RolActual,
            cancellationToken);
    }
}

public class AsignarIncidenteCasoUso(
    IIncidenteRepository incidenteRepository,
    IUsuarioRepository usuarioRepository,
    IRegistroAuditoriaRepository registroAuditoriaRepository,
    IUnitOfWork unitOfWork,
    IAuditoriaService auditoriaService)
{
    private readonly IIncidenteRepository _incidenteRepository = incidenteRepository;
    private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;
    private readonly IRegistroAuditoriaRepository _registroAuditoriaRepository = registroAuditoriaRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IAuditoriaService _auditoriaService = auditoriaService;

    public async Task<IncidenteDetalleDto> EjecutarAsync(AsignarIncidenteSolicitud solicitud, CancellationToken cancellationToken = default)
    {
        IncidenteCasoUsoHelper.ValidarEsSoporte(solicitud.RolActual);

        var incidente = await _incidenteRepository.ObtenerPorIdAsync(solicitud.IncidenteId, cancellationToken)
            ?? throw new InvalidOperationException("No se encontro el incidente.");

        IncidenteCasoUsoHelper.ValidarIncidenteOperable(incidente);

        var agente = await _usuarioRepository.ObtenerPorIdAsync(solicitud.AgenteId, cancellationToken)
            ?? throw new InvalidOperationException("No se encontro el agente seleccionado.");

        IncidenteCasoUsoHelper.ValidarUsuarioActivo(agente);

        if (!IncidenteCasoUsoHelper.EsSupervisor(agente.Rol?.Nombre))
        {
            throw new InvalidOperationException("El usuario asignado debe tener rol Supervisor.");
        }

        var accion = incidente.AgenteAsignadoId.HasValue
            ? TipoAccionAuditoria.ReasignacionAgente
            : TipoAccionAuditoria.AsignacionAgente;

        var valorAnterior = incidente.AgenteAsignado?.Nombre ?? "Sin asignar";

        incidente.AgenteAsignadoId = agente.Id;
        incidente.AgenteAsignado = agente;
        incidente.FechaActualizacion = DateTime.UtcNow;

        if (incidente.Estado == EstadoIncidente.Nuevo)
        {
            incidente.Estado = EstadoIncidente.Asignado;
        }

        await _registroAuditoriaRepository.AgregarAsync(
            _auditoriaService.CrearRegistro(
                incidente.Id,
                solicitud.UsuarioActualId,
                accion,
                "Incidente asignado a un agente.",
                valorAnterior,
                agente.Nombre),
            cancellationToken);

        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        return await IncidenteCasoUsoHelper.ObtenerDetalleAutorizadoAsync(
            _incidenteRepository,
            incidente.Id,
            solicitud.UsuarioActualId,
            solicitud.RolActual,
            cancellationToken);
    }
}

public class CambiarEstadoIncidenteCasoUso(
    IIncidenteRepository incidenteRepository,
    IRegistroAuditoriaRepository registroAuditoriaRepository,
    IUnitOfWork unitOfWork,
    IAuditoriaService auditoriaService)
{
    private readonly IIncidenteRepository _incidenteRepository = incidenteRepository;
    private readonly IRegistroAuditoriaRepository _registroAuditoriaRepository = registroAuditoriaRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IAuditoriaService _auditoriaService = auditoriaService;

    public async Task<IncidenteDetalleDto> EjecutarAsync(CambiarEstadoIncidenteSolicitud solicitud, CancellationToken cancellationToken = default)
    {
        IncidenteCasoUsoHelper.ValidarEsSoporte(solicitud.RolActual);

        var incidente = await _incidenteRepository.ObtenerPorIdAsync(solicitud.IncidenteId, cancellationToken)
            ?? throw new InvalidOperationException("No se encontro el incidente.");

        IncidenteCasoUsoHelper.ValidarIncidenteOperable(incidente);
        IncidenteCasoUsoHelper.ValidarCambioEstado(incidente.Estado, solicitud.NuevoEstado);

        var estadoAnterior = incidente.Estado;

        incidente.Estado = solicitud.NuevoEstado;
        incidente.FechaActualizacion = DateTime.UtcNow;

        await _registroAuditoriaRepository.AgregarAsync(
            _auditoriaService.CrearRegistro(
                incidente.Id,
                solicitud.UsuarioActualId,
                TipoAccionAuditoria.CambioEstado,
                "Estado del incidente actualizado.",
                estadoAnterior.ToString(),
                solicitud.NuevoEstado.ToString()),
            cancellationToken);

        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        return await IncidenteCasoUsoHelper.ObtenerDetalleAutorizadoAsync(
            _incidenteRepository,
            incidente.Id,
            solicitud.UsuarioActualId,
            solicitud.RolActual,
            cancellationToken);
    }
}

public class AgregarComentarioIncidenteCasoUso(
    IIncidenteRepository incidenteRepository,
    IComentarioIncidenteRepository comentarioIncidenteRepository,
    IRegistroAuditoriaRepository registroAuditoriaRepository,
    IUnitOfWork unitOfWork,
    ISlaService slaService,
    IAuditoriaService auditoriaService)
{
    private readonly IIncidenteRepository _incidenteRepository = incidenteRepository;
    private readonly IComentarioIncidenteRepository _comentarioIncidenteRepository = comentarioIncidenteRepository;
    private readonly IRegistroAuditoriaRepository _registroAuditoriaRepository = registroAuditoriaRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ISlaService _slaService = slaService;
    private readonly IAuditoriaService _auditoriaService = auditoriaService;

    public async Task<IncidenteDetalleDto> EjecutarAsync(AgregarComentarioIncidenteSolicitud solicitud, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(solicitud.Contenido))
        {
            throw new InvalidOperationException("El comentario no puede estar vacio.");
        }

        var incidente = await _incidenteRepository.ObtenerPorIdAsync(solicitud.IncidenteId, cancellationToken)
            ?? throw new InvalidOperationException("No se encontro el incidente.");

        IncidenteCasoUsoHelper.ValidarIncidenteOperable(incidente);
        IncidenteCasoUsoHelper.ValidarPuedeComentar(incidente, solicitud.UsuarioActualId, solicitud.RolActual, solicitud.EsInterno);

        var fechaComentario = DateTime.UtcNow;
        var comentario = new ComentarioIncidente
        {
            IncidenteId = incidente.Id,
            UsuarioId = solicitud.UsuarioActualId,
            Contenido = solicitud.Contenido.Trim(),
            EsInterno = solicitud.EsInterno,
            FechaCreacion = fechaComentario
        };

        if (!solicitud.EsInterno && IncidenteCasoUsoHelper.EsSoporte(solicitud.RolActual))
        {
            _slaService.RegistrarPrimeraRespuestaSiCorresponde(incidente, fechaComentario);
        }

        incidente.FechaActualizacion = fechaComentario;

        await _comentarioIncidenteRepository.AgregarAsync(comentario, cancellationToken);
        await _registroAuditoriaRepository.AgregarAsync(
            _auditoriaService.CrearRegistro(
                incidente.Id,
                solicitud.UsuarioActualId,
                TipoAccionAuditoria.ComentarioAgregado,
                solicitud.EsInterno ? "Comentario interno agregado." : "Comentario publico agregado."),
            cancellationToken);

        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        return await IncidenteCasoUsoHelper.ObtenerDetalleAutorizadoAsync(
            _incidenteRepository,
            incidente.Id,
            solicitud.UsuarioActualId,
            solicitud.RolActual,
            cancellationToken);
    }
}

public class ResolverIncidenteCasoUso(
    IIncidenteRepository incidenteRepository,
    IRegistroAuditoriaRepository registroAuditoriaRepository,
    IUnitOfWork unitOfWork,
    ISlaService slaService,
    IAuditoriaService auditoriaService)
{
    private readonly IIncidenteRepository _incidenteRepository = incidenteRepository;
    private readonly IRegistroAuditoriaRepository _registroAuditoriaRepository = registroAuditoriaRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ISlaService _slaService = slaService;
    private readonly IAuditoriaService _auditoriaService = auditoriaService;

    public async Task<IncidenteDetalleDto> EjecutarAsync(ResolverIncidenteSolicitud solicitud, CancellationToken cancellationToken = default)
    {
        IncidenteCasoUsoHelper.ValidarEsSoporte(solicitud.RolActual);

        var incidente = await _incidenteRepository.ObtenerPorIdAsync(solicitud.IncidenteId, cancellationToken)
            ?? throw new InvalidOperationException("No se encontro el incidente.");

        IncidenteCasoUsoHelper.ValidarIncidenteOperable(incidente);

        var fechaResolucion = DateTime.UtcNow;
        var estadoAnterior = incidente.Estado;

        _slaService.RegistrarPrimeraRespuestaSiCorresponde(incidente, fechaResolucion);

        incidente.Estado = EstadoIncidente.Resuelto;
        incidente.FechaResolucion = fechaResolucion;
        incidente.FechaActualizacion = fechaResolucion;

        await _registroAuditoriaRepository.AgregarAsync(
            _auditoriaService.CrearRegistro(
                incidente.Id,
                solicitud.UsuarioActualId,
                TipoAccionAuditoria.Resolucion,
                "Incidente resuelto.",
                estadoAnterior.ToString(),
                EstadoIncidente.Resuelto.ToString()),
            cancellationToken);

        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        return await IncidenteCasoUsoHelper.ObtenerDetalleAutorizadoAsync(
            _incidenteRepository,
            incidente.Id,
            solicitud.UsuarioActualId,
            solicitud.RolActual,
            cancellationToken);
    }
}

public class CerrarIncidenteCasoUso(
    IIncidenteRepository incidenteRepository,
    IRegistroAuditoriaRepository registroAuditoriaRepository,
    IUnitOfWork unitOfWork,
    IAuditoriaService auditoriaService)
{
    private readonly IIncidenteRepository _incidenteRepository = incidenteRepository;
    private readonly IRegistroAuditoriaRepository _registroAuditoriaRepository = registroAuditoriaRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IAuditoriaService _auditoriaService = auditoriaService;

    public async Task<IncidenteDetalleDto> EjecutarAsync(CerrarIncidenteSolicitud solicitud, CancellationToken cancellationToken = default)
    {
        var incidente = await _incidenteRepository.ObtenerPorIdAsync(solicitud.IncidenteId, cancellationToken)
            ?? throw new InvalidOperationException("No se encontro el incidente.");

        IncidenteCasoUsoHelper.ValidarPuedeCerrar(incidente, solicitud.UsuarioActualId, solicitud.RolActual);

        if (incidente.Estado != EstadoIncidente.Resuelto)
        {
            throw new InvalidOperationException("Solo se puede cerrar un incidente resuelto.");
        }

        incidente.Estado = EstadoIncidente.Cerrado;
        incidente.FechaCierre = DateTime.UtcNow;
        incidente.FechaActualizacion = incidente.FechaCierre;

        await _registroAuditoriaRepository.AgregarAsync(
            _auditoriaService.CrearRegistro(
                incidente.Id,
                solicitud.UsuarioActualId,
                TipoAccionAuditoria.Cierre,
                "Incidente cerrado.",
                EstadoIncidente.Resuelto.ToString(),
                EstadoIncidente.Cerrado.ToString()),
            cancellationToken);

        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        return await IncidenteCasoUsoHelper.ObtenerDetalleAutorizadoAsync(
            _incidenteRepository,
            incidente.Id,
            solicitud.UsuarioActualId,
            solicitud.RolActual,
            cancellationToken);
    }
}

internal static class IncidenteCasoUsoHelper
{
    public static bool EsAdministrador(string? rol) =>
        string.Equals(rol, RolesSistema.Administrador, StringComparison.OrdinalIgnoreCase);

    public static bool EsSupervisor(string? rol) =>
        string.Equals(rol, RolesSistema.Supervisor, StringComparison.OrdinalIgnoreCase);

    public static bool EsSolicitante(string? rol) =>
        string.Equals(rol, RolesSistema.Solicitante, StringComparison.OrdinalIgnoreCase);

    public static bool EsSoporte(string? rol) => EsAdministrador(rol) || EsSupervisor(rol);

    public static void ValidarUsuarioActivo(Usuario usuario)
    {
        if (!usuario.Activo)
        {
            throw new UnauthorizedAccessException("El usuario se encuentra inactivo.");
        }
    }

    public static void ValidarEsSoporte(string rolActual)
    {
        if (!EsSoporte(rolActual))
        {
            throw new UnauthorizedAccessException("No tienes permisos para ejecutar esta accion.");
        }
    }

    public static bool PuedeVerIncidente(Incidente incidente, int usuarioActualId, string rolActual)
    {
        if (EsSoporte(rolActual))
        {
            return true;
        }

        return incidente.SolicitanteId == usuarioActualId;
    }

    public static void ValidarIncidenteOperable(Incidente incidente)
    {
        if (!incidente.Activo || incidente.Estado is EstadoIncidente.Cerrado or EstadoIncidente.Cancelado)
        {
            throw new InvalidOperationException("El incidente ya no admite cambios.");
        }
    }

    public static void ValidarCambioEstado(EstadoIncidente estadoActual, EstadoIncidente nuevoEstado)
    {
        if (estadoActual == nuevoEstado)
        {
            throw new InvalidOperationException("El incidente ya se encuentra en ese estado.");
        }

        if (nuevoEstado is EstadoIncidente.Resuelto or EstadoIncidente.Cerrado or EstadoIncidente.Cancelado)
        {
            throw new InvalidOperationException("Usa el caso de uso especializado para resolver, cerrar o cancelar incidentes.");
        }
    }

    public static void ValidarPuedeComentar(Incidente incidente, int usuarioActualId, string rolActual, bool esInterno)
    {
        if (esInterno)
        {
            if (!EsSoporte(rolActual))
            {
                throw new UnauthorizedAccessException("Solo soporte puede crear comentarios internos.");
            }

            return;
        }

        if (EsSoporte(rolActual))
        {
            return;
        }

        if (incidente.SolicitanteId != usuarioActualId)
        {
            throw new UnauthorizedAccessException("No tienes permisos para comentar este incidente.");
        }
    }

    public static void ValidarPuedeCerrar(Incidente incidente, int usuarioActualId, string rolActual)
    {
        if (EsSoporte(rolActual))
        {
            return;
        }

        if (EsSolicitante(rolActual) && incidente.SolicitanteId == usuarioActualId)
        {
            return;
        }

        throw new UnauthorizedAccessException("No tienes permisos para cerrar este incidente.");
    }

    public static async Task<IncidenteDetalleDto> ObtenerDetalleAutorizadoAsync(
        IIncidenteRepository incidenteRepository,
        int incidenteId,
        int usuarioActualId,
        string rolActual,
        CancellationToken cancellationToken)
    {
        var incidente = await incidenteRepository.ObtenerConDetalleAsync(incidenteId, cancellationToken)
            ?? throw new InvalidOperationException("No se encontro el incidente.");

        if (!PuedeVerIncidente(incidente, usuarioActualId, rolActual))
        {
            throw new UnauthorizedAccessException("No tienes permisos para ver este incidente.");
        }

        return MapearDetalle(incidente, usuarioActualId, rolActual);
    }

    public static IncidenteResumenDto MapearResumen(Incidente incidente)
    {
        var ahoraUtc = DateTime.UtcNow;
        var primeraRespuestaVencida = incidente.FechaPrimeraRespuesta is null
            && ahoraUtc > incidente.FechaLimitePrimeraRespuesta
            && incidente.Estado is not EstadoIncidente.Cerrado
            && incidente.Estado is not EstadoIncidente.Cancelado;

        var resolucionVencida = incidente.FechaResolucion is null
            && incidente.FechaCierre is null
            && ahoraUtc > incidente.FechaLimiteResolucion
            && incidente.Estado is not EstadoIncidente.Cancelado;

        return new IncidenteResumenDto(
            incidente.Id,
            incidente.Codigo,
            incidente.Titulo,
            incidente.Estado.ToString(),
            incidente.Prioridad.ToString(),
            incidente.SolicitanteId,
            incidente.Solicitante?.Nombre ?? string.Empty,
            incidente.AgenteAsignadoId,
            incidente.AgenteAsignado?.Nombre,
            incidente.PoliticaSLA?.Nombre ?? string.Empty,
            incidente.FechaCreacion,
            incidente.FechaLimitePrimeraRespuesta,
            incidente.FechaLimiteResolucion,
            incidente.FechaPrimeraRespuesta,
            incidente.FechaResolucion,
            incidente.FechaCierre,
            primeraRespuestaVencida,
            resolucionVencida);
    }

    public static IncidenteDetalleDto MapearDetalle(Incidente incidente, int usuarioActualId, string rolActual)
    {
        var ahoraUtc = DateTime.UtcNow;
        var primeraRespuestaVencida = incidente.FechaPrimeraRespuesta is null
            && ahoraUtc > incidente.FechaLimitePrimeraRespuesta
            && incidente.Estado is not EstadoIncidente.Cerrado
            && incidente.Estado is not EstadoIncidente.Cancelado;

        var resolucionVencida = incidente.FechaResolucion is null
            && incidente.FechaCierre is null
            && ahoraUtc > incidente.FechaLimiteResolucion
            && incidente.Estado is not EstadoIncidente.Cancelado;

        var comentarios = incidente.Comentarios
            .Where(x => !x.EsInterno || EsSoporte(rolActual))
            .OrderBy(x => x.FechaCreacion)
            .Select(x => new ComentarioIncidenteDto(
                x.Id,
                x.Contenido,
                x.EsInterno,
                x.UsuarioId,
                x.Usuario?.Nombre ?? string.Empty,
                x.FechaCreacion))
            .ToArray();

        var politicaSla = incidente.PoliticaSLA
            ?? throw new InvalidOperationException("El incidente no tiene una politica SLA asociada.");

        var puedeAgregarComentarioPublico = incidente.Estado is not EstadoIncidente.Cerrado
            && incidente.Estado is not EstadoIncidente.Cancelado
            && (EsSoporte(rolActual) || incidente.SolicitanteId == usuarioActualId);

        var puedeAgregarComentarioInterno = incidente.Estado is not EstadoIncidente.Cerrado
            && incidente.Estado is not EstadoIncidente.Cancelado
            && EsSoporte(rolActual);

        return new IncidenteDetalleDto(
            incidente.Id,
            incidente.Codigo,
            incidente.Titulo,
            incidente.Descripcion,
            incidente.Estado.ToString(),
            incidente.Prioridad.ToString(),
            incidente.SolicitanteId,
            incidente.Solicitante?.Nombre ?? string.Empty,
            incidente.AgenteAsignadoId,
            incidente.AgenteAsignado?.Nombre,
            new PoliticaSlaDto(
                politicaSla.Id,
                politicaSla.Nombre,
                politicaSla.Descripcion,
                politicaSla.Prioridad.ToString(),
                politicaSla.TiempoPrimeraRespuestaMinutos,
                politicaSla.TiempoResolucionMinutos,
                politicaSla.Activo),
            incidente.Activo,
            incidente.FechaCreacion,
            incidente.FechaActualizacion,
            incidente.FechaPrimeraRespuesta,
            incidente.FechaLimitePrimeraRespuesta,
            incidente.FechaLimiteResolucion,
            incidente.FechaResolucion,
            incidente.FechaCierre,
            primeraRespuestaVencida,
            resolucionVencida,
            puedeAgregarComentarioPublico,
            puedeAgregarComentarioInterno,
            comentarios);
    }
}
