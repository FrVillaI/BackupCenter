namespace BackupCenter.Application.Services;

public class JwtSettings
{
    public string Issuer { get; set; } = "BackupCenter";
    public string Audience { get; set; } = "BackupCenterClient";
    public string SecretKey { get; set; } = "CHANGE_ME_SecretKey_ReplaceInProd";
    public int ExpiryHours { get; set; } = 24;
}
