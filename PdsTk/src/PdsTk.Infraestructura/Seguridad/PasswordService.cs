using PdsTk.Aplicacion.Abstracciones.Seguridad;

namespace PdsTk.Infraestructura.Seguridad;

public class PasswordService : IPasswordService
{
    public string GenerarHash(string textoPlano) => PasswordHelper.GenerarHash(textoPlano);

    public bool VerificarHash(string textoPlano, string hashExistente) =>
        PasswordHelper.VerificarHash(textoPlano, hashExistente);
}
