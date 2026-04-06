using PdsTk.Aplicacion.Dtos;
using PdsTk.Dominio.Entidades;

namespace PdsTk.Aplicacion.Abstracciones.Persistencia;

public interface IIncidenteRepository
{
    Task AgregarAsync(Incidente incidente, CancellationToken cancellationToken = default);

    Task<Incidente?> ObtenerPorIdAsync(int incidenteId, CancellationToken cancellationToken = default);

    Task<Incidente?> ObtenerConDetalleAsync(int incidenteId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Incidente>> ListarAsync(ConsultaIncidentesFiltro filtro, CancellationToken cancellationToken = default);
}
