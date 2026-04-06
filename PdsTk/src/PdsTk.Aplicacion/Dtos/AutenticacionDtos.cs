namespace PdsTk.Aplicacion.Dtos;

public record IniciarSesionSolicitud(string Correo, string Clave);

public record TokenGeneradoDto(string Token, DateTime ExpiraEnUtc);

public record PerfilUsuarioDto(int Id, string Nombre, string Correo, string Rol);

public record SesionDto(string Token, DateTime ExpiraEnUtc, PerfilUsuarioDto Usuario);
