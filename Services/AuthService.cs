using System.Security.Cryptography;
using System.Text;

namespace Journal.Services;

public class AuthService
{
    private readonly DatabaseService _database;
    private bool _isAuthenticated = false;
    private const string PasswordHashKey = "password_hash";

    public AuthService(DatabaseService database)
    {
        _database = database;
    }

    public bool IsAuthenticated => _isAuthenticated;

    public async Task<bool> HasPasswordSetAsync()
    {
        var hash = await _database.GetSettingAsync(PasswordHashKey);
        return !string.IsNullOrEmpty(hash);
    }

    public async Task<bool> SetPasswordAsync(string password)
    {
        var hash = HashPassword(password);
        await _database.SetSettingAsync(PasswordHashKey, hash);
        _isAuthenticated = true;
        return true;
    }

    public async Task<bool> VerifyPasswordAsync(string password)
    {
        var storedHash = await _database.GetSettingAsync(PasswordHashKey);
        if (string.IsNullOrEmpty(storedHash))
            return false;

        var inputHash = HashPassword(password);
        var isValid = storedHash == inputHash;

        if (isValid)
            _isAuthenticated = true;

        return isValid;
    }

    public void Logout()
    {
        _isAuthenticated = false;
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}