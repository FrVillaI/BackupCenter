namespace BackupCenter.Application.Services;

/// Configuración para la generación y validación de tokens JWT.
/// Se carga desde appsettings.json.
public class JwtSettings
{
    /// Configuración para la generación y validación de tokens JWT.
    /// Se carga desde appsettings.json.
    public string Issuer { get; set; } = "BackupCenter";

    /// Emisor del token (Issuer).
    /// Identifica quién generó el JWT.
    public string Audience { get; set; } = "BackupCenterClient";

    /// Clave secreta utilizada para firmar el token.
    /// Debe ser segura y no debe subirse a repositorios públicos.
    public string SecretKey { get; set; } = "CHANGE_ME_SecretKey_ReplaceInProd";

    /// Tiempo de expiración del token en horas.
    public int ExpiryHours { get; set; } = 24;
}
