namespace PdsTk.Dominio.Entidades;

public class ComentarioIncidente
{
    public int Id { get; set; }

    public string Contenido { get; set; } = string.Empty;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public bool EsInterno { get; set; } = false;

    // Incidente
    public int IncidenteId { get; set; }

    public Incidente? Incidente { get; set; }

    // Usuario
    public int UsuarioId { get; set; }

    public Usuario? Usuario { get; set; }
}