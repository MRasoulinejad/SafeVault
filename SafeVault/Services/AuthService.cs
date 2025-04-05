using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SafeVault.Models;
using BCrypt.Net;

namespace SafeVault.Services;

public class AuthService
{
    private readonly string _key = "ThisIsASecretKeyForJWTGeneration123!"; // TODO: store safely
    private readonly UserService _userService;

    public AuthService(UserService userService)
    {
        _userService = userService;
    }

    public bool Register(string username, string email, string password, string role = "user")
    {
        if (_userService.UserExists(username)) return false;
        var hashed = BCrypt.Net.BCrypt.HashPassword(password);
        return _userService.AddUser(username, email, hashed, role);
    }

    public string? Login(string username, string password)
    {
        var user = _userService.GetUserByUsername(username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public TokenValidationParameters GetValidationParameters() =>
        new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
            ValidateIssuerSigningKey = true
        };
}
