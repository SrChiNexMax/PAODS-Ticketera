using PdsTk.Dominio.Entidades;
using PdsTk.Dominio.Enumeradores;

namespace PdsTk.Aplicacion.Servicios;

public interface ISlaService
{
    Task<PoliticaSLA> ObtenerPoliticaActivaPorPrioridadAsync(PrioridadIncidente prioridad, CancellationToken cancellationToken = default);

    (DateTime fechaLimitePrimeraRespuesta, DateTime fechaLimiteResolucion) CalcularLimites(DateTime fechaBaseUtc, PoliticaSLA politicaSla);

    void RegistrarPrimeraRespuestaSiCorresponde(Incidente incidente, DateTime fechaRespuestaUtc);
}
