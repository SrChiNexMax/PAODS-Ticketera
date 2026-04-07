using System.Security.Claims;

namespace PdsTk.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int ObtenerUsuarioId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(claim, out var usuarioId))
        {
            throw new UnauthorizedAccessException("Token invalido.");
        }

        return usuarioId;
    }

    public static string ObtenerRol(this ClaimsPrincipal user)
    {
        var rol = user.FindFirstValue(ClaimTypes.Role);

        if (string.IsNullOrWhiteSpace(rol))
        {
            throw new UnauthorizedAccessException("Token invalido.");
        }

        return rol;
    }
}
