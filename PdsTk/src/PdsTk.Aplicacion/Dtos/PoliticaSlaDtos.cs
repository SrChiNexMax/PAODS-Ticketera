namespace PdsTk.Aplicacion.Dtos;

public record PoliticaSlaDto(
    int Id,
    string Nombre,
    string Descripcion,
    string Prioridad,
    int TiempoPrimeraRespuestaMinutos,
    int TiempoResolucionMinutos,
    bool Activo);
