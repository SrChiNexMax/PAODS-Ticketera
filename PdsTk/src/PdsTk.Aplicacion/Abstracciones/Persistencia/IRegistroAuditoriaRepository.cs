using PdsTk.Dominio.Entidades;

namespace PdsTk.Aplicacion.Abstracciones.Persistencia;

public interface IRegistroAuditoriaRepository
{
    Task AgregarAsync(RegistroAuditoria registro, CancellationToken cancellationToken = default);
}
