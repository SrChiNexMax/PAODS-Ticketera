using System.Security.Cryptography;
using System.Text;

namespace PdsTk.Infraestructura.Seguridad;

public static class PasswordHelper
{
    public static string GenerarHash(string textoPlano)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(textoPlano);
        var hashBytes = sha256.ComputeHash(bytes);
        return Convert.ToHexString(hashBytes);
    }
}