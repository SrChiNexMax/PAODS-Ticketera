using PdsTk.Dominio.Enumeradores;

namespace PdsTk.Dominio.Entidades;


public class RegistroAuditoria
{
    public int Id { get; set; }

    public int IncidenteId { get; set; }
    public Incidente? Incidente { get; set; }

    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public TipoAccionAuditoria TipoAccion { get; set; }

    public string Descripcion { get; set; } = string.Empty;

    public string? ValorAnterior { get; set; }

    public string? ValorNuevo { get; set; }

    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}