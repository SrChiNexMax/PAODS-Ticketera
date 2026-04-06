using PdsTk.Dominio.Entidades;
using PdsTk.Dominio.Enumeradores;

namespace PdsTk.Aplicacion.Servicios;

public class AuditoriaService : IAuditoriaService
{
    public RegistroAuditoria CrearRegistro(
        int incidenteId,
        int usuarioId,
        TipoAccionAuditoria tipoAccion,
        string descripcion,
        string? valorAnterior = null,
        string? valorNuevo = null)
    {
        return new RegistroAuditoria
        {
            IncidenteId = incidenteId,
            UsuarioId = usuarioId,
            TipoAccion = tipoAccion,
            Descripcion = descripcion,
            ValorAnterior = valorAnterior,
            ValorNuevo = valorNuevo,
            Fecha = DateTime.UtcNow
        };
    }
}
