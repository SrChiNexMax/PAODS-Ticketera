using PdsTk.Aplicacion.Dtos;
using PdsTk.Dominio.Entidades;

namespace PdsTk.Aplicacion.Abstracciones.Seguridad;

public interface ITokenJwtService
{
    TokenGeneradoDto GenerarToken(Usuario usuario);
}
