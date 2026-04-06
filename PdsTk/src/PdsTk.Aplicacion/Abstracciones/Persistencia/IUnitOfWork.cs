namespace PdsTk.Aplicacion.Abstracciones.Persistencia;

public interface IUnitOfWork
{
    Task<int> GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
