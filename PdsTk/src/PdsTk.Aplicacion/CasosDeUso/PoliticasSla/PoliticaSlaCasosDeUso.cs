using PdsTk.Aplicacion.Abstracciones.Persistencia;
using PdsTk.Aplicacion.Dtos;

namespace PdsTk.Aplicacion.CasosDeUso.PoliticasSla;

public class ListarPoliticasSlaCasoUso(IPoliticaSlaRepository politicaSlaRepository)
{
    private readonly IPoliticaSlaRepository _politicaSlaRepository = politicaSlaRepository;

    public async Task<IReadOnlyCollection<PoliticaSlaDto>> EjecutarAsync(CancellationToken cancellationToken = default)
    {
        var politicas = await _politicaSlaRepository.ListarActivasAsync(cancellationToken);

        return politicas
            .OrderBy(x => x.Prioridad)
            .Select(x => new PoliticaSlaDto(
                x.Id,
                x.Nombre,
                x.Descripcion,
                x.Prioridad.ToString(),
                x.TiempoPrimeraRespuestaMinutos,
                x.TiempoResolucionMinutos,
                x.Activo))
            .ToArray();
    }
}
