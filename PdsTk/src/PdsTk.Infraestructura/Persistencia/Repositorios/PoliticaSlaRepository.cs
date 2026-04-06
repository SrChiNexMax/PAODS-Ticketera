using Microsoft.EntityFrameworkCore;
using PdsTk.Aplicacion.Abstracciones.Persistencia;
using PdsTk.Dominio.Entidades;
using PdsTk.Dominio.Enumeradores;

namespace PdsTk.Infraestructura.Persistencia.Repositorios;

public class PoliticaSlaRepository(PdsTkDbContext context) : IPoliticaSlaRepository
{
    private readonly PdsTkDbContext _context = context;

    public Task<PoliticaSLA?> ObtenerActivaPorPrioridadAsync(PrioridadIncidente prioridad, CancellationToken cancellationToken = default)
    {
        return _context.PoliticasSLA
            .FirstOrDefaultAsync(x => x.Activo && x.Prioridad == prioridad, cancellationToken);
    }

    public async Task<IReadOnlyCollection<PoliticaSLA>> ListarActivasAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PoliticasSLA
            .AsNoTracking()
            .Where(x => x.Activo)
            .OrderBy(x => x.Prioridad)
            .ToArrayAsync(cancellationToken);
    }
}
