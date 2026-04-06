using PdsTk.Dominio.Entidades;

namespace PdsTk.Aplicacion.Abstracciones.Persistencia;

public interface IComentarioIncidenteRepository
{
    Task AgregarAsync(ComentarioIncidente comentario, CancellationToken cancellationToken = default);
}
