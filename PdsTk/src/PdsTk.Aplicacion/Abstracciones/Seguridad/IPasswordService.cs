namespace PdsTk.Aplicacion.Abstracciones.Seguridad;

public interface IPasswordService
{
    string GenerarHash(string textoPlano);

    bool VerificarHash(string textoPlano, string hashExistente);
}
