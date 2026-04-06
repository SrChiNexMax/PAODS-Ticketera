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

    public static bool VerificarHash(string textoPlano, string hashExistente)
    {
        var hashGenerado = GenerarHash(textoPlano);
        return string.Equals(hashGenerado, hashExistente, StringComparison.OrdinalIgnoreCase);
    }
}
