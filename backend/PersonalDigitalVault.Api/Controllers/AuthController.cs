using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PersonalDigitalVault.Api.DTOs.Auth;
using PersonalDigitalVault.Api.Interfaces.Services;

namespace PersonalDigitalVault.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;


    public AuthController(
        IAuthService authService)
    {
        _authService =
            authService;
    }


    // =====================================================
    // REGISTER
    // =====================================================
    //
    // POST:
    // /api/auth/register
    //
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterRequestDto request)
    {
        var result =
            await _authService
                .RegisterAsync(
                    request
                );


        if (result == null)
        {
            return BadRequest(
                new
                {
                    message =
                        "Email already registered."
                }
            );
        }


        return Ok(
            result
        );
    }


    // =====================================================
    // VERIFY EMAIL OTP
    // =====================================================
    //
    // POST:
    // /api/auth/verify-email
    //
    // SUCCESS:
    //
    // EmailVerified = true
    //
    // SetupToken = temporary JWT
    //
    [AllowAnonymous]
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail(
        VerifyEmailOtpRequestDto request)
    {
        var result =
            await _authService
                .VerifyEmailOtpAsync(
                    request
                );


        if (result == null)
        {
            return BadRequest(
                new
                {
                    message =
                        "Invalid, expired, or maximum OTP attempts reached."
                }
            );
        }


        return Ok(
            result
        );
    }


    // =====================================================
    // RESEND EMAIL OTP
    // =====================================================
    //
    // POST:
    // /api/auth/resend-email-otp
    //
    [AllowAnonymous]
    [HttpPost("resend-email-otp")]
    public async Task<IActionResult> ResendEmailOtp(
        ResendEmailOtpRequestDto request)
    {
        var success =
            await _authService
                .ResendEmailOtpAsync(
                    request
                );


        if (!success)
        {
            return BadRequest(
                new
                {
                    message =
                        "Unable to resend OTP. Wait 60 seconds or check account status."
                }
            );
        }


        return Ok(
            new
            {
                message =
                    "New verification code sent successfully."
            }
        );
    }


    // =====================================================
    // SETUP GOOGLE AUTHENTICATOR / TOTP
    // =====================================================
    //
    // POST:
    // /api/auth/setup-totp
    //
    // INPUT:
    //
    // SetupToken
    //
    // OUTPUT:
    //
    // SecretKey
    // OtpAuthUri
    // QrCodeDataUrl
    //
    [AllowAnonymous]
    [HttpPost("setup-totp")]
    public async Task<IActionResult> SetupTotp(
        SetupTotpRequestDto request)
    {
        var result =
            await _authService
                .SetupTotpAsync(
                    request
                );


        if (result == null)
        {
            return BadRequest(
                new
                {
                    message =
                        "Invalid or expired TOTP setup token."
                }
            );
        }


        return Ok(
            result
        );
    }


    // =====================================================
    // VERIFY GOOGLE AUTHENTICATOR SETUP
    // =====================================================
    //
    // POST:
    // /api/auth/verify-totp-setup
    //
    // INPUT:
    //
    // SetupToken
    // 6 digit Authenticator Code
    //
    [AllowAnonymous]
    [HttpPost("verify-totp-setup")]
    public async Task<IActionResult> VerifyTotpSetup(
        VerifyTotpSetupRequestDto request)
    {
        var success =
            await _authService
                .VerifyTotpSetupAsync(
                    request
                );


        if (!success)
        {
            return BadRequest(
                new
                {
                    message =
                        "Invalid authenticator code or expired setup session."
                }
            );
        }


        return Ok(
            new
            {
                message =
                    "Two-factor authentication enabled successfully."
            }
        );
    }


    // =====================================================
    // LOGIN
    // =====================================================
    //
    // POST:
    // /api/auth/login
    //
    // IMPORTANT:
    //
    // Next Phase-la normal client login
    // Password + TOTP flow-a change pannuvom.
    //
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginRequestDto request)
    {
        var result =
            await _authService
                .LoginAsync(
                    request
                );


        if (result == null)
        {
            return Unauthorized(
                new
                {
                    message =
                        "Invalid login or additional verification is required."
                }
            );
        }


        return Ok(
            result
        );
    }
    // =========================================================
    // VERIFY LOGIN TOTP
    // =========================================================
    //
    // POST:
    // /api/auth/verify-login-totp
    //
    // INPUT:
    // ChallengeToken
    // Authenticator 6-digit code
    //
    // SUCCESS:
    // FINAL JWT
    //
    [AllowAnonymous]
    [HttpPost("verify-login-totp")]
    public async Task<IActionResult>
        VerifyLoginTotp(
            VerifyLoginTotpRequestDto request)
    {
        var result =
            await _authService
                .VerifyLoginTotpAsync(
                    request
                );


        if (result == null)
        {
            return Unauthorized(
                new
                {
                    message =
                        "Invalid or expired authenticator verification."
                }
            );
        }


        return Ok(
            result
        );
    }

    // =====================================================
    // LOGOUT
    // =====================================================
    //
    // POST:
    // /api/auth/logout
    //
    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return NoContent();
    }
}