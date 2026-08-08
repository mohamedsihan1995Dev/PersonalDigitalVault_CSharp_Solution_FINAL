//using Microsoft.IdentityModel.Tokens; using PersonalDigitalVault.Api.Entities; using System.IdentityModel.Tokens.Jwt; using System.Security.Claims; using System.Text;
//namespace PersonalDigitalVault.Api.Security;
//public class JwtTokenGenerator(IConfiguration configuration)
//{
//    public string Generate(User user){var key=configuration["Jwt:Key"]??throw new InvalidOperationException("JWT key missing");var claims=new[]{new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),new Claim(ClaimTypes.Name,user.FullName),new Claim(ClaimTypes.Email,user.Email),new Claim(ClaimTypes.Role,user.Role)};var creds=new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),SecurityAlgorithms.HmacSha256);var minutes=int.TryParse(configuration["Jwt:ExpiryMinutes"],out var m)?m:60;var token=new JwtSecurityToken(configuration["Jwt:Issuer"],configuration["Jwt:Audience"],claims,expires:DateTime.UtcNow.AddMinutes(minutes),signingCredentials:creds);return new JwtSecurityTokenHandler().WriteToken(token);}
//}


using Microsoft.IdentityModel.Tokens;
using PersonalDigitalVault.Api.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PersonalDigitalVault.Api.Security;

public class JwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // INPUT:
    // user -> login successful aana User object
    //
    // REASON:
    // User/Admin identity + role JWT token-kulla store panna.
    //
    // OUTPUT:
    // JWT token string return pannum.
    public string GenerateToken(User user)
    {
        var key = _configuration["Jwt:Key"];

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new InvalidOperationException(
                "JWT secret key configure pannala.");
        }

        var claims = new List<Claim>
        {
            // User Id
            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()),

            // User name
            new Claim(
                ClaimTypes.Name,
                user.FullName),

            // User email
            new Claim(
                ClaimTypes.Email,
                user.Email),

            // IMPORTANT
            // User / Admin role token-kulla pogum.
            new Claim(
                ClaimTypes.Role,
                user.Role)
        };

        var securityKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key));

        var credentials =
            new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

        var expiryMinutes =
            int.TryParse(
                _configuration["Jwt:ExpiryMinutes"],
                out var minutes)
                ? minutes
                : 120;

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}