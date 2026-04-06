using Microsoft.EntityFrameworkCore;
using PdsTk.Aplicacion.Abstracciones.Persistencia;
using PdsTk.Dominio.Entidades;

namespace PdsTk.Infraestructura.Persistencia.Repositorios;

public class UsuarioRepository(PdsTkDbContext context) : IUsuarioRepository
{
    private readonly PdsTkDbContext _context = context;

    public async Task<Usuario?> ObtenerPorCorreoAsync(string correo, CancellationToken cancellationToken = default)
    {
        var correoNormalizado = correo.Trim().ToLowerInvariant();

        return await _context.Usuarios
            .Include(x => x.Rol)
            .FirstOrDefaultAsync(x => x.Correo.ToLower() == correoNormalizado, cancellationToken);
    }

    public Task<Usuario?> ObtenerPorIdAsync(int usuarioId, CancellationToken cancellationToken = default)
    {
        return _context.Usuarios
            .Include(x => x.Rol)
            .FirstOrDefaultAsync(x => x.Id == usuarioId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Usuario>> ListarPorRolAsync(string rolNombre, CancellationToken cancellationToken = default)
    {
        return await _context.Usuarios
            .AsNoTracking()
            .Include(x => x.Rol)
            .Where(x => x.Activo && x.Rol != null && x.Rol.Nombre == rolNombre)
            .OrderBy(x => x.Nombre)
            .ToArrayAsync(cancellationToken);
    }
}
