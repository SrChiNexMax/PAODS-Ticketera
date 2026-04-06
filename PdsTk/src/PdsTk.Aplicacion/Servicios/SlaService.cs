using PdsTk.Aplicacion.Abstracciones.Persistencia;
using PdsTk.Dominio.Entidades;
using PdsTk.Dominio.Enumeradores;

namespace PdsTk.Aplicacion.Servicios;

public class SlaService(IPoliticaSlaRepository politicaSlaRepository) : ISlaService
{
    private readonly IPoliticaSlaRepository _politicaSlaRepository = politicaSlaRepository;

    public async Task<PoliticaSLA> ObtenerPoliticaActivaPorPrioridadAsync(PrioridadIncidente prioridad, CancellationToken cancellationToken = default)
    {
        var politica = await _politicaSlaRepository.ObtenerActivaPorPrioridadAsync(prioridad, cancellationToken);

        return politica ?? throw new InvalidOperationException($"No existe una politica SLA activa para la prioridad {prioridad}.");
    }

    public (DateTime fechaLimitePrimeraRespuesta, DateTime fechaLimiteResolucion) CalcularLimites(DateTime fechaBaseUtc, PoliticaSLA politicaSla)
    {
        var fechaLimitePrimeraRespuesta = fechaBaseUtc.AddMinutes(politicaSla.TiempoPrimeraRespuestaMinutos);
        var fechaLimiteResolucion = fechaBaseUtc.AddMinutes(politicaSla.TiempoResolucionMinutos);

        return (fechaLimitePrimeraRespuesta, fechaLimiteResolucion);
    }

    public void RegistrarPrimeraRespuestaSiCorresponde(Incidente incidente, DateTime fechaRespuestaUtc)
    {
        if (incidente.FechaPrimeraRespuesta.HasValue)
        {
            return;
        }

        incidente.FechaPrimeraRespuesta = fechaRespuestaUtc;
    }
}
