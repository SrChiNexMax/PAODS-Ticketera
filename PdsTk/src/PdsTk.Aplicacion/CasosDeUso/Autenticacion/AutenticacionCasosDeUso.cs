using PdsTk.Aplicacion.Abstracciones.Persistencia;
using PdsTk.Aplicacion.Abstracciones.Seguridad;
using PdsTk.Aplicacion.Dtos;

namespace PdsTk.Aplicacion.CasosDeUso.Autenticacion;

public class IniciarSesionCasoUso(
    IUsuarioRepository usuarioRepository,
    IPasswordService passwordService,
    ITokenJwtService tokenJwtService)
{
    private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;
    private readonly IPasswordService _passwordService = passwordService;
    private readonly ITokenJwtService _tokenJwtService = tokenJwtService;

    public async Task<SesionDto> EjecutarAsync(IniciarSesionSolicitud solicitud, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(solicitud.Correo) || string.IsNullOrWhiteSpace(solicitud.Clave))
        {
            throw new UnauthorizedAccessException("Las credenciales son obligatorias.");
        }

        var usuario = await _usuarioRepository.ObtenerPorCorreoAsync(solicitud.Correo.Trim(), cancellationToken)
            ?? throw new UnauthorizedAccessException("Credenciales invalidas.");

        if (!usuario.Activo)
        {
            throw new UnauthorizedAccessException("El usuario se encuentra inactivo.");
        }

        if (!_passwordService.VerificarHash(solicitud.Clave, usuario.Clave))
        {
            throw new UnauthorizedAccessException("Credenciales invalidas.");
        }

        if (usuario.Rol is null)
        {
            throw new InvalidOperationException("El usuario no tiene un rol asociado.");
        }

        var tokenGenerado = _tokenJwtService.GenerarToken(usuario);
        var perfil = new PerfilUsuarioDto(usuario.Id, usuario.Nombre, usuario.Correo, usuario.Rol.Nombre);

        return new SesionDto(tokenGenerado.Token, tokenGenerado.ExpiraEnUtc, perfil);
    }
}

public class ObtenerPerfilActualCasoUso(IUsuarioRepository usuarioRepository)
{
    private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;

    public async Task<PerfilUsuarioDto> EjecutarAsync(int usuarioId, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.ObtenerPorIdAsync(usuarioId, cancellationToken)
            ?? throw new UnauthorizedAccessException("No se encontro el usuario autenticado.");

        if (!usuario.Activo)
        {
            throw new UnauthorizedAccessException("El usuario se encuentra inactivo.");
        }

        if (usuario.Rol is null)
        {
            throw new InvalidOperationException("El usuario no tiene un rol asociado.");
        }

        return new PerfilUsuarioDto(usuario.Id, usuario.Nombre, usuario.Correo, usuario.Rol.Nombre);
    }
}
