using PdsTk.Aplicacion.Abstracciones.Persistencia;
using PdsTk.Dominio.Entidades;

namespace PdsTk.Infraestructura.Persistencia.Repositorios;

public class RegistroAuditoriaRepository(PdsTkDbContext context) : IRegistroAuditoriaRepository
{
    private readonly PdsTkDbContext _context = context;

    public Task AgregarAsync(RegistroAuditoria registro, CancellationToken cancellationToken = default)
    {
        return _context.RegistrosAuditoria.AddAsync(registro, cancellationToken).AsTask();
    }
}
