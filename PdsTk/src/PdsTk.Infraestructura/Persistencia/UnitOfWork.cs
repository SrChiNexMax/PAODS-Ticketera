using PdsTk.Aplicacion.Abstracciones.Persistencia;

namespace PdsTk.Infraestructura.Persistencia;

public class UnitOfWork(PdsTkDbContext context) : IUnitOfWork
{
    private readonly PdsTkDbContext _context = context;

    public Task<int> GuardarCambiosAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
