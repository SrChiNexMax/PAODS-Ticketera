namespace PdsTk.Dominio.Entidades;

public class Usuario
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Correo { get; set; } = string.Empty;

    public string Clave { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public int RolId { get; set; }

    public Rol? Rol { get; set; }

    public ICollection<Incidente> IncidentesAsignados { get; set; } = new List<Incidente>();

    public ICollection<Incidente> IncidentesSolicitados { get; set; } = new List<Incidente>();

    public ICollection<RegistroAuditoria> RegistroAuditoria { get; set; } = new List<RegistroAuditoria>();

    public ICollection<ComentarioIncidente> Comentarios { get; set; } = new List<ComentarioIncidente>();

}