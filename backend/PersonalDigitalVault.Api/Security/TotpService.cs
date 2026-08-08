using OtpNet;
using QRCoder;

namespace PersonalDigitalVault.Api.Security;

public class TotpService
{
    private const string Issuer =
        "Personal Digital Vault";


    // =====================================================
    // TOTP SECRET GENERATE
    // =====================================================
    //
    // OUTPUT:
    // Random Base32 secret
    //
    // Google Authenticator indha secret use panni
    // 30 seconds-ku oru OTP generate pannum.
    //
    public string GenerateSecret()
    {
        var secretBytes =
            KeyGeneration.GenerateRandomKey(
                20
            );


        return Base32Encoding.ToString(
            secretBytes
        );
    }


    // =====================================================
    // OTPAUTH URI GENERATE
    // =====================================================
    //
    // INPUT:
    // secret
    // email
    //
    // OUTPUT:
    // Google Authenticator understand pannura URI
    //
    public string GenerateOtpAuthUri(
        string secret,
        string email)
    {
        var label =
            Uri.EscapeDataString(
                $"{Issuer}:{email}"
            );


        var encodedIssuer =
            Uri.EscapeDataString(
                Issuer
            );


        return
            $"otpauth://totp/{label}" +
            $"?secret={secret}" +
            $"&issuer={encodedIssuer}" +
            $"&digits=6" +
            $"&period=30";
    }


    // =====================================================
    // QR CODE GENERATE
    // =====================================================
    //
    // INPUT:
    // otpauth URI
    //
    // OUTPUT:
    // Base64 PNG image
    //
    // Frontend:
    // <img src="data:image/png;base64,...">
    //
    public string GenerateQrCodeDataUrl(
        string otpAuthUri)
    {
        using var generator =
            new QRCodeGenerator();


        using var qrData =
            generator.CreateQrCode(
                otpAuthUri,
                QRCodeGenerator.ECCLevel.Q
            );


        using var qrCode =
            new PngByteQRCode(
                qrData
            );


        var qrBytes =
            qrCode.GetGraphic(
                20
            );


        return
            "data:image/png;base64," +
            Convert.ToBase64String(
                qrBytes
            );
    }


    // =====================================================
    // VERIFY TOTP CODE
    // =====================================================
    //
    // INPUT:
    // secret
    // Google Authenticator 6 digit code
    //
    // OUTPUT:
    // true / false
    //
    public bool VerifyCode(
        string secret,
        string code)
    {
        if (
            string.IsNullOrWhiteSpace(secret) ||
            string.IsNullOrWhiteSpace(code)
        )
        {
            return false;
        }


        // Spaces remove pannum
        var cleanCode =
            code
                .Replace(" ", "")
                .Trim();


        if (
            cleanCode.Length != 6 ||
            !cleanCode.All(char.IsDigit)
        )
        {
            return false;
        }


        var secretBytes =
            Base32Encoding.ToBytes(
                secret
            );


        var totp =
            new Totp(
                secretBytes,

                // 30 second interval
                step: 30,

                // Google Authenticator standard
                mode:
                    OtpHashMode.Sha1,

                // 6 digit OTP
                totpSize: 6
            );


        // Small clock difference tolerate pannum
        return totp.VerifyTotp(
            cleanCode,
            out _,
            VerificationWindow
                .RfcSpecifiedNetworkDelay
        );
    }
}