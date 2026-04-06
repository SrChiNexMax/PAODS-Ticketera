using PdsTk.Dominio.Entidades;
using PdsTk.Dominio.Enumeradores;

namespace PdsTk.Aplicacion.Abstracciones.Persistencia;

public interface IPoliticaSlaRepository
{
    Task<PoliticaSLA?> ObtenerActivaPorPrioridadAsync(PrioridadIncidente prioridad, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<PoliticaSLA>> ListarActivasAsync(CancellationToken cancellationToken = default);
}
