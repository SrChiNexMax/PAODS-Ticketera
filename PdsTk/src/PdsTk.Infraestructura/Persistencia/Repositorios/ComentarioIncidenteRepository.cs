using PdsTk.Aplicacion.Abstracciones.Persistencia;
using PdsTk.Dominio.Entidades;

namespace PdsTk.Infraestructura.Persistencia.Repositorios;

public class ComentarioIncidenteRepository(PdsTkDbContext context) : IComentarioIncidenteRepository
{
    private readonly PdsTkDbContext _context = context;

    public Task AgregarAsync(ComentarioIncidente comentario, CancellationToken cancellationToken = default)
    {
        return _context.ComentariosIncidente.AddAsync(comentario, cancellationToken).AsTask();
    }
}
