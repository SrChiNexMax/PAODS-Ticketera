namespace PdsTk.Infraestructura.Seguridad;

public class JwtOptions
{
    public string Issuer { get; init; } = string.Empty;

    public string Audience { get; init; } = string.Empty;

    public string SecretKey { get; init; } = string.Empty;

    public int ExpiracionMinutos { get; init; } = 120;
}
