using PdsTk.Dominio.Entidades;
using PdsTk.Dominio.Enumeradores;

namespace PdsTk.Aplicacion.Servicios;

public interface IAuditoriaService
{
    RegistroAuditoria CrearRegistro(
        int incidenteId,
        int usuarioId,
        TipoAccionAuditoria tipoAccion,
        string descripcion,
        string? valorAnterior = null,
        string? valorNuevo = null);
}
