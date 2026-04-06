namespace PdsTk.Aplicacion.Servicios;

public class CodigoIncidenteService : ICodigoIncidenteService
{
    public string GenerarCodigo()
    {
        var marcaTiempo = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var sufijo = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
        return $"INC-{marcaTiempo}-{sufijo}";
    }
}
