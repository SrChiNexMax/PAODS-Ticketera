using Microsoft.EntityFrameworkCore;
using PdsTk.Aplicacion.Abstracciones.Persistencia;
using PdsTk.Aplicacion.Dtos;
using PdsTk.Dominio.Entidades;

namespace PdsTk.Infraestructura.Persistencia.Repositorios;

public class IncidenteRepository(PdsTkDbContext context) : IIncidenteRepository
{
    private readonly PdsTkDbContext _context = context;

    public Task AgregarAsync(Incidente incidente, CancellationToken cancellationToken = default)
    {
        return _context.Incidentes.AddAsync(incidente, cancellationToken).AsTask();
    }

    public Task<Incidente?> ObtenerPorIdAsync(int incidenteId, CancellationToken cancellationToken = default)
    {
        return _context.Incidentes
            .Include(x => x.Solicitante)
            .ThenInclude(x => x!.Rol)
            .Include(x => x.AgenteAsignado)
            .ThenInclude(x => x!.Rol)
            .Include(x => x.PoliticaSLA)
            .FirstOrDefaultAsync(x => x.Id == incidenteId, cancellationToken);
    }

    public Task<Incidente?> ObtenerConDetalleAsync(int incidenteId, CancellationToken cancellationToken = default)
    {
        return _context.Incidentes
            .AsNoTracking()
            .Include(x => x.Solicitante)
            .ThenInclude(x => x!.Rol)
            .Include(x => x.AgenteAsignado)
            .ThenInclude(x => x!.Rol)
            .Include(x => x.PoliticaSLA)
            .Include(x => x.Comentarios.OrderBy(c => c.FechaCreacion))
            .ThenInclude(x => x.Usuario)
            .Include(x => x.RegistrosAuditoria)
            .ThenInclude(x => x.Usuario)
            .FirstOrDefaultAsync(x => x.Id == incidenteId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Incidente>> ListarAsync(ConsultaIncidentesFiltro filtro, CancellationToken cancellationToken = default)
    {
        var query = _context.Incidentes
            .AsNoTracking()
            .Include(x => x.Solicitante)
            .Include(x => x.AgenteAsignado)
            .Include(x => x.PoliticaSLA)
            .AsQueryable();

        if (filtro.Estado.HasValue)
        {
            query = query.Where(x => x.Estado == filtro.Estado.Value);
        }

        if (filtro.Prioridad.HasValue)
        {
            query = query.Where(x => x.Prioridad == filtro.Prioridad.Value);
        }

        if (filtro.AgenteAsignadoId.HasValue)
        {
            query = query.Where(x => x.AgenteAsignadoId == filtro.AgenteAsignadoId.Value);
        }

        if (filtro.SolicitanteId.HasValue)
        {
            query = query.Where(x => x.SolicitanteId == filtro.SolicitanteId.Value);
        }

        if (filtro.SoloActivos)
        {
            query = query.Where(x => x.Activo);
        }

        return await query
            .OrderByDescending(x => x.FechaCreacion)
            .ToArrayAsync(cancellationToken);
    }
}
