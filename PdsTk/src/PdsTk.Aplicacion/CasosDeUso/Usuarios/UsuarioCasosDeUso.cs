using PdsTk.Aplicacion.Abstracciones.Persistencia;
using PdsTk.Aplicacion.Dtos;
using PdsTk.Dominio.Constantes;

namespace PdsTk.Aplicacion.CasosDeUso.Usuarios;

public class ListarAgentesCasoUso(IUsuarioRepository usuarioRepository)
{
    private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;

    public async Task<IReadOnlyCollection<UsuarioResumenDto>> EjecutarAsync(CancellationToken cancellationToken = default)
    {
        var usuarios = await _usuarioRepository.ListarPorRolAsync(RolesSistema.Agente, cancellationToken);

        return usuarios
            .Select(x => new UsuarioResumenDto(
                x.Id,
                x.Nombre,
                x.Correo,
                x.Rol?.Nombre ?? RolesSistema.Agente))
            .ToArray();
    }
}
