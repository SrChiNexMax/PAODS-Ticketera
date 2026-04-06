using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PdsTk.Aplicacion.Abstracciones.Seguridad;
using PdsTk.Aplicacion.Dtos;
using PdsTk.Dominio.Entidades;

namespace PdsTk.Infraestructura.Seguridad;

public class JwtTokenService(JwtOptions jwtOptions) : ITokenJwtService
{
    private readonly JwtOptions _jwtOptions = jwtOptions;

    public TokenGeneradoDto GenerarToken(Usuario usuario)
    {
        if (usuario.Rol is null)
        {
            throw new InvalidOperationException("El usuario no tiene rol asociado.");
        }

        if (string.IsNullOrWhiteSpace(_jwtOptions.SecretKey))
        {
            throw new InvalidOperationException("La configuracion JWT no contiene una SecretKey valida.");
        }

        var expiracion = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiracionMinutos);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nombre),
            new(ClaimTypes.Email, usuario.Correo),
            new(ClaimTypes.Role, usuario.Rol.Nombre)
        };

        var tokenDescriptor = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: expiracion,
            signingCredentials: credentials);

        var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        return new TokenGeneradoDto(token, expiracion);
    }
}
