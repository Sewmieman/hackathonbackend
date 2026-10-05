using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FixMyCampus.Api.Data;
using FixMyCampus.Api.DTOs;
using FixMyCampus.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
namespace FixMyCampus.Api.Services;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public AuthService(
        AppDbContext context,
        IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<(bool Success, string Message, LoginResponseDto? Data)> RegisterAsync(
        RegisterDto dto)
    {
        var email = dto.Email.Trim().ToLower();

        var existingUser = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == email);

        if (existingUser != null)
        {
            return (false, "Email already exists.", null);
        }

        var user = new User
        {
            FullName = dto.FullName.Trim(),
            Email = email,
            Role = dto.Role
        };

        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            dto.Password);

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        var token = GenerateToken(user);

        var response = new LoginResponseDto(
            user.Id,
            user.FullName,
            user.Email,
            user.Role,
            token
        );

        return (true, "Registration successful.", response);
    }

    public async Task<(bool Success, string Message, LoginResponseDto? Data)> LoginAsync(
        LoginDto dto)
    {
        var email = dto.Email.Trim().ToLower();

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == email);

        if (user == null)
        {
            return (false, "Invalid email or password.", null);
        }

        if (user.Role != dto.Role)
        {
            return (false, "Selected role does not match your account.", null);
        }

        var passwordResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            dto.Password);

        if (passwordResult == PasswordVerificationResult.Failed)
        {
            return (false, "Invalid email or password.", null);
        }

        var token = GenerateToken(user);

        var response = new LoginResponseDto(
            user.Id,
            user.FullName,
            user.Email,
            user.Role,
            token
        );

        return (true, "Login successful.", response);
    }

    private string GenerateToken(User user)
    {
        var jwt = _configuration.GetSection("Jwt");

        var key = jwt["Key"]
            ?? throw new InvalidOperationException("JWT Key is missing.");

        var issuer = jwt["Issuer"];
        var audience = jwt["Audience"];

        var expiresMinutes = int.TryParse(
            jwt["ExpiresMinutes"],
            out var minutes)
            ? minutes
            : 60;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),

            new(ClaimTypes.NameIdentifier, user.Id.ToString()),

            new(ClaimTypes.Name, user.FullName),

            new(ClaimTypes.Email, user.Email),

            new(ClaimTypes.Role, user.Role.ToString())
        };

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key));

        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}