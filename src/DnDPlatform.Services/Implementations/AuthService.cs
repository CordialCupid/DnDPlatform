using DnDPlatform.Models.Domain;
using DnDPlatform.Models.DTOs.Auth;
using DnDPlatform.Repositories.Interfaces;
using DnDPlatform.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DnDPlatform.Services.Implementations;

public class AuthService(IUserRepository userRepo, IConfiguration config) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await userRepo.ExistsAsync(request.Username, request.Email))
            throw new InvalidOperationException("Username or email already in use.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        await userRepo.InsertAsync(user);
        return GenerateToken(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await userRepo.GetByUsernameAsync(request.Username)
            ?? throw new UnauthorizedAccessException("Invalid credentials.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        return GenerateToken(user);
    }

    private AuthResponse GenerateToken(User user)
    {
        var jwtSettings = config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddHours(double.Parse(jwtSettings["ExpiresHours"] ?? "24"));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        return new AuthResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Username = user.Username,
            UserId = user.Id,
            ExpiresAt = expires
        };
    }
}
