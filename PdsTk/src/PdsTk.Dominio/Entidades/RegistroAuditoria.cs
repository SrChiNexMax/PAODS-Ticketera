using PdsTk.Dominio.Enumeradores;

namespace PdsTk.Dominio.Entidades;

public class RegistroAuditoria
{
    public int Id { get; set; }

    public TipoAccionAuditoria TipoAccion { get; set; }

    public string Descripcion { get; set; } = string.Empty;

    public string? ValorAnterior { get; set; }

    public string? ValorNuevo { get; set; }

    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    public string Usuario { get; set; } = string.Empty;

    // Incidente
    public int IncidenteId { get; set; }
    public Incidente? Incidente { get; set; }

    // Usuario
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

}
