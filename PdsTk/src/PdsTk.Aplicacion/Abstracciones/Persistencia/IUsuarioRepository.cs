using PdsTk.Dominio.Entidades;

namespace PdsTk.Aplicacion.Abstracciones.Persistencia;

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorCorreoAsync(string correo, CancellationToken cancellationToken = default);

    Task<Usuario?> ObtenerPorIdAsync(int usuarioId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Usuario>> ListarPorRolAsync(string rolNombre, CancellationToken cancellationToken = default);
}
