using System.Security.Cryptography;
using System.Text;

namespace PersonalDigitalVault.Api.Security;

public class EmailOtpService
{
    private readonly IConfiguration _configuration;

    public EmailOtpService(
        IConfiguration configuration)
    {
        _configuration = configuration;
    }


    // =====================================================
    // OTP GENERATE
    // =====================================================
    //
    // OUTPUT:
    // Secure random 6 digit OTP
    //
    // Example:
    // 482731
    //
    public string GenerateOtp()
    {
        var number =
            RandomNumberGenerator.GetInt32(
                0,
                1_000_000
            );

        return number.ToString("D6");
    }


    // =====================================================
    // OTP HASH
    // =====================================================
    //
    // INPUT:
    // Plain 6 digit OTP
    //
    // REASON:
    // Plain OTP database-la store panna koodathu.
    //
    // OUTPUT:
    // Secure HMAC SHA256 hash
    //
    public string HashOtp(
        string otp)
    {
        var pepper =
            _configuration[
                "Security:OtpPepper"
            ];

        if (string.IsNullOrWhiteSpace(
            pepper))
        {
            throw new InvalidOperationException(
                "OTP security key is missing."
            );
        }


        var keyBytes =
            Encoding.UTF8.GetBytes(
                pepper
            );

        var otpBytes =
            Encoding.UTF8.GetBytes(
                otp
            );


        using var hmac =
            new HMACSHA256(
                keyBytes
            );


        var hash =
            hmac.ComputeHash(
                otpBytes
            );


        return Convert.ToBase64String(
            hash
        );
    }


    // =====================================================
    // OTP VERIFY
    // =====================================================
    //
    // INPUT:
    // User enter panna OTP
    // Database stored hash
    //
    // OUTPUT:
    // true  -> correct
    // false -> wrong
    //
    public bool VerifyOtp(
        string otp,
        string storedHash)
    {
        if (
            string.IsNullOrWhiteSpace(otp) ||
            string.IsNullOrWhiteSpace(storedHash)
        )
        {
            return false;
        }


        var newHash =
            HashOtp(
                otp.Trim()
            );


        var newHashBytes =
            Convert.FromBase64String(
                newHash
            );


        var storedHashBytes =
            Convert.FromBase64String(
                storedHash
            );


        return CryptographicOperations
            .FixedTimeEquals(
                newHashBytes,
                storedHashBytes
            );
    }
}