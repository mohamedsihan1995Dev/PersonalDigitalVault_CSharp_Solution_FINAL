using PersonalDigitalVault.Api.DTOs.Auth;

namespace PersonalDigitalVault.Api.Interfaces.Services;

public interface IAuthService
{
    // Register + Email OTP
    Task<RegisterResponseDto?> RegisterAsync(
        RegisterRequestDto request);


    // Email OTP verify
    Task<EmailVerificationResponseDto?>
        VerifyEmailOtpAsync(
            VerifyEmailOtpRequestDto request);


    // Resend Email OTP
    Task<bool> ResendEmailOtpAsync(
        ResendEmailOtpRequestDto request);


    // Generate Authenticator QR
    Task<TotpSetupResponseDto?>
        SetupTotpAsync(
            SetupTotpRequestDto request);


    // First Authenticator code verify
    Task<bool> VerifyTotpSetupAsync(
        VerifyTotpSetupRequestDto request);


    // =====================================================
    // LOGIN STEP 1
    // =====================================================
    //
    // Admin:
    // Password correct → JWT
    //
    // Client:
    // Password correct → MFA Challenge Token
    //
    Task<MfaChallengeResponseDto?>
        LoginAsync(
            LoginRequestDto request);


    // =====================================================
    // LOGIN STEP 2
    // =====================================================
    //
    // Client TOTP verify
    // correct-na FINAL JWT
    //
    Task<LoginResponseDto?>
        VerifyLoginTotpAsync(
            VerifyLoginTotpRequestDto request);
}