using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.Tokens;

using PersonalDigitalVault.Api.Entities;

namespace PersonalDigitalVault.Api.Security;

public class MfaChallengeTokenService
{
    private readonly IConfiguration _configuration;


    public MfaChallengeTokenService(
        IConfiguration configuration)
    {
        _configuration = configuration;
    }


    // =====================================================
    // LOGIN MFA CHALLENGE TOKEN GENERATE
    // =====================================================
    //
    // INPUT:
    // Password correct-a verify panna User
    //
    // REASON:
    // Password verify pannina odane final JWT kudukka koodathu.
    //
    // OUTPUT:
    // 5 minutes valid temporary MFA token
    //
    public string GenerateChallengeToken(
        User user)
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
                "JWT Key missing."
            );
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
                new Claim(
                    "userId",
                    user.Id.ToString()
                ),

                new Claim(
                    "purpose",
                    "login_mfa"
                ),

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

                // Challenge 5 minutes mattum valid
                expires:
                    DateTime.UtcNow
                        .AddMinutes(5),

                signingCredentials:
                    credentials
            );


        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }


    // =====================================================
    // LOGIN MFA TOKEN VALIDATE
    // =====================================================
    //
    // INPUT:
    // Challenge token
    //
    // OUTPUT:
    // Valid-na UserId
    // Invalid / Expired-na null
    //
    public int? ValidateChallengeToken(
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

                    ValidIssuer =
                        _configuration[
                            "Jwt:Issuer"
                        ],

                    ValidateAudience = true,

                    ValidAudience =
                        _configuration[
                            "Jwt:Audience"
                        ],

                    ValidateIssuerSigningKey =
                        true,

                    IssuerSigningKey =
                        key,

                    ValidateLifetime =
                        true,

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


            // Token registration setup token-a
            // login-ku use panna koodathu.
            var purpose =
                principal.FindFirst(
                    "purpose"
                )?.Value;


            if (purpose != "login_mfa")
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
            return null;
        }
    }
}