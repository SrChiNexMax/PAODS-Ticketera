using PdsTk.Dominio.Enumeradores;

namespace PdsTk.Dominio.Entidades;

public class Incidente 
{
    public int Id { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string Codigo { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public EstadoIncidente Estado { get; set; } = EstadoIncidente.Nuevo;
    
    public PrioridadIncidente Prioridad { get; set; } = PrioridadIncidente.Media;

    //Solicitante
    public int SolicitanteId { get; set; }

    public Usuario? Solicitante { get; set; }

    //Asignado
    public int? AgenteAsignadoId { get; set; }

    public Usuario? AgenteAsignado { get; set; }

    //Política de SLA
    public int PoliticaSLAId { get; set; }

    public PoliticaSLA? PoliticaSLA { get; set; }

    public bool Activo { get; set; } = true;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public DateTime? FechaPrimeraRespuesta { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public DateTime? FechaCierre { get; set; }

    public DateTime FechaLimitePrimeraRespuesta { get; set; }

    public DateTime FechaLimiteResolucion { get; set; }
    
    public DateTime? FechaResolucion { get; set; }

    public ICollection<ComentarioIncidente> Comentarios { get; set; } = new List<ComentarioIncidente>();

    public ICollection<RegistroAuditoria> RegistrosAuditoria { get; set; } = new List<RegistroAuditoria>();
}