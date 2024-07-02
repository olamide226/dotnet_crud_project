using Microsoft.AspNetCore.Identity;

namespace complaint_api.Common;
public class HashUtils
{
    private readonly PasswordHasher<string> _passwordHasher;

    private readonly string _passwordSalt = "somesalt";

    public HashUtils()
    {
        _passwordHasher = new PasswordHasher<string>();
    }

    public string HashPassword(string password)
    {
        return _passwordHasher.HashPassword(_passwordSalt, password);
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword(_passwordSalt, hashedPassword, providedPassword);
        return result == PasswordVerificationResult.Success;
    }
}
