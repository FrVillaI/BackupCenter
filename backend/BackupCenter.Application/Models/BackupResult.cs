namespace BackupCenter.Application.Models;

public class BackupResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public string ZipPath { get; set; } = string.Empty;
    public string Hash { get; set; } = string.Empty;
    public string HashPath { get; set; } = string.Empty;
}