using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PersonalDigitalVault.Api.Entities;

namespace PersonalDigitalVault.Api.Security;

public class RegistrationTokenService
{
    private readonly IConfiguration _configuration;

    public RegistrationTokenService(
        IConfiguration configuration)
    {
        _configuration = configuration;
    }


    // =====================================================
    // TOTP SETUP TOKEN GENERATE
    // =====================================================
    //
    // INPUT:
    // Email verify panna User
    //
    // REASON:
    // Email mattum use panni TOTP setup panna allow panna koodathu.
    //
    // OUTPUT:
    // 10 minutes valid temporary setup token
    //
    // IMPORTANT:
    // Idhu normal application JWT illa.
    //
    public string GenerateTotpSetupToken(User user)
    {
        var jwtKey =
            _configuration["Jwt:Key"];

        var issuer =
            _configuration["Jwt:Issuer"];

        var audience =
            _configuration["Jwt:Audience"];


        if (string.IsNullOrWhiteSpace(jwtKey))
        {
            throw new InvalidOperationException(
                "JWT Key missing.");
        }


        var key =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            );


        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );


        var claims =
            new List<Claim>
            {
                // எந்த user-ku token
                new Claim(
                    "userId",
                    user.Id.ToString()
                ),

                // Token purpose
                new Claim(
                    "purpose",
                    "totp_setup"
                ),

                // User email
                new Claim(
                    ClaimTypes.Email,
                    user.Email
                )
            };


        var token =
            new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,

                // 10 minutes mattum valid
                expires:
                    DateTime.UtcNow
                        .AddMinutes(10),

                signingCredentials:
                    credentials
            );


        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }


    // =====================================================
    // TOTP SETUP TOKEN VALIDATE
    // =====================================================
    //
    // INPUT:
    // Temporary setup token
    //
    // OUTPUT:
    // Valid-na UserId
    // Invalid / Expired-na null
    //
    public int? ValidateTotpSetupToken(
        string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }


        try
        {
            var jwtKey =
                _configuration["Jwt:Key"];

            var issuer =
                _configuration["Jwt:Issuer"];

            var audience =
                _configuration["Jwt:Audience"];


            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                return null;
            }


            var key =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey)
                );


            var parameters =
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,

                    ValidateAudience = true,
                    ValidAudience = audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,

                    ValidateLifetime = true,

                    ClockSkew =
                        TimeSpan.FromSeconds(30)
                };


            var handler =
                new JwtSecurityTokenHandler();


            var principal =
                handler.ValidateToken(
                    token,
                    parameters,
                    out _
                );


            // Correct purpose-a check pannum
            var purpose =
                principal.FindFirst(
                    "purpose"
                )?.Value;


            if (purpose != "totp_setup")
            {
                return null;
            }


            var userIdText =
                principal.FindFirst(
                    "userId"
                )?.Value;


            if (
                !int.TryParse(
                    userIdText,
                    out var userId
                )
            )
            {
                return null;
            }


            return userId;
        }
        catch
        {
            // Expired / modified / invalid token
            return null;
        }
    }
}