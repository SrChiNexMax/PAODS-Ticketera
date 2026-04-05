using PdsTk.Dominio.Enumeradores;

namespace PdsTk.Dominio.Entidades;

public class PoliticaSLA
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public PrioridadIncidente Prioridad { get; set; }

    public int TiempoPrimeraRespuestaMinutos { get; set; }

    public int TiempoResolucionMinutos { get; set; }

    public bool Activo { get; set; } = true;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public ICollection<Incidente> Incidentes { get; set; } = new List<Incidente>();
}