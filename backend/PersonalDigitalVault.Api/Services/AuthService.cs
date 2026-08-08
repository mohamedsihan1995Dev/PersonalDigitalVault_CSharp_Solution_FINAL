using PersonalDigitalVault.Api.DTOs.Auth;
using PersonalDigitalVault.Api.Entities;
using PersonalDigitalVault.Api.Interfaces.Repositories;
using PersonalDigitalVault.Api.Interfaces.Services;
using PersonalDigitalVault.Api.Security;

namespace PersonalDigitalVault.Api.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    private readonly PasswordHasher _passwordHasher;

    private readonly JwtTokenGenerator _jwtTokenGenerator;

    private readonly EmailOtpService _emailOtpService;

    private readonly IEmailService _emailService;

    private readonly RegistrationTokenService _registrationTokenService;

    private readonly TotpService _totpService;

    private readonly TotpSecretProtector _totpSecretProtector;

    // =====================================================
    // STEP 6
    // LOGIN MFA CHALLENGE TOKEN SERVICE
    // =====================================================
    //
    // Password correct aana piragu
    // temporary 5-minute MFA token create panna use pannuvom.
    //
    private readonly MfaChallengeTokenService
        _mfaChallengeTokenService;


    // =====================================================
    // CONSTRUCTOR
    // =====================================================
    //
    // Required services ellam Dependency Injection
    // moolama inga varum.
    //
    public AuthService(
        IUserRepository userRepository,
        PasswordHasher passwordHasher,
        JwtTokenGenerator jwtTokenGenerator,
        EmailOtpService emailOtpService,
        IEmailService emailService,
        RegistrationTokenService registrationTokenService,
        TotpService totpService,
        TotpSecretProtector totpSecretProtector,

        // STEP 6
        MfaChallengeTokenService mfaChallengeTokenService)
    {
        _userRepository =
            userRepository;

        _passwordHasher =
            passwordHasher;

        _jwtTokenGenerator =
            jwtTokenGenerator;

        _emailOtpService =
            emailOtpService;

        _emailService =
            emailService;

        _registrationTokenService =
            registrationTokenService;

        _totpService =
            totpService;

        _totpSecretProtector =
            totpSecretProtector;

        // STEP 6
        _mfaChallengeTokenService =
            mfaChallengeTokenService;
    }


    // =====================================================
    // REGISTER USER
    // =====================================================
    //
    // FLOW:
    //
    // Email check
    //      ↓
    // User create
    //      ↓
    // Password hash
    //      ↓
    // 6 digit OTP generate
    //      ↓
    // OTP hash store
    //      ↓
    // Email send
    //
    public async Task<RegisterResponseDto?> RegisterAsync(
        RegisterRequestDto request)
    {
        var email =
            request.Email
                .Trim()
                .ToLowerInvariant();


        // =================================================
        // EMAIL ALREADY EXISTS CHECK
        // =================================================

        var existingUser =
            await _userRepository
                .GetByEmailAsync(
                    email
                );


        if (existingUser != null)
        {
            return null;
        }


        // =================================================
        // EMAIL OTP GENERATE
        // =================================================

        var otp =
            _emailOtpService
                .GenerateOtp();


        // Plain OTP database-la save panna maatom.
        var otpHash =
            _emailOtpService
                .HashOtp(
                    otp
                );


        // =================================================
        // USER CREATE
        // =================================================

        var user =
            new User
            {
                FullName =
                    request.FullName.Trim(),

                Email =
                    email,

                Role =
                    "User",

                IsActive =
                    true,

                CreatedAt =
                    DateTime.UtcNow,

                // Email innum verify aagala
                IsEmailVerified =
                    false,

                // OTP hash
                EmailOtpHash =
                    otpHash,

                // OTP 5 minutes valid
                EmailOtpExpiresAt =
                    DateTime.UtcNow
                        .AddMinutes(5),

                EmailOtpAttempts =
                    0,

                EmailOtpLastSentAt =
                    DateTime.UtcNow,

                // Authenticator innum setup aagala
                IsTotpEnabled =
                    false,

                TotpSecretEncrypted =
                    null
            };


        // =================================================
        // PASSWORD HASH
        // =================================================

        user.PasswordHash =
            _passwordHasher
                .HashPassword(
                    user,
                    request.Password
                );


        // =================================================
        // SAVE USER
        // =================================================

        await _userRepository
            .AddAsync(
                user
            );


        // =================================================
        // SEND EMAIL OTP
        // =================================================

        await _emailService
            .SendVerificationOtpAsync(
                user.Email,
                user.FullName,
                otp
            );


        // JWT inga return panna maatom.
        return new RegisterResponseDto
        {
            UserId =
                user.Id,

            Email =
                user.Email,

            RequiresEmailVerification =
                true,

            Message =
                "Verification code sent to your email."
        };
    }


    // =====================================================
    // VERIFY EMAIL OTP
    // =====================================================
    //
    // INPUT:
    //
    // Email
    // OTP
    //
    // SUCCESS:
    //
    // IsEmailVerified = true
    //
    // SetupToken generate pannum.
    //
    public async Task<EmailVerificationResponseDto?>
        VerifyEmailOtpAsync(
            VerifyEmailOtpRequestDto request)
    {
        var email =
            request.Email
                .Trim()
                .ToLowerInvariant();


        // User find
        var user =
            await _userRepository
                .GetByEmailAsync(
                    email
                );


        if (user == null)
        {
            return null;
        }


        // =================================================
        // ALREADY VERIFIED
        // =================================================
        //
        // Email already verified-a irundhaalum
        // TOTP setup complete aagala na
        // new temporary setup token kudukkalaam.
        //
        if (user.IsEmailVerified)
        {
            var existingSetupToken =
                _registrationTokenService
                    .GenerateTotpSetupToken(
                        user
                    );


            return new EmailVerificationResponseDto
            {
                EmailVerified =
                    true,

                SetupToken =
                    existingSetupToken,

                Message =
                    "Email already verified."
            };
        }


        // =================================================
        // OTP DATA CHECK
        // =================================================

        if (
            string.IsNullOrWhiteSpace(
                user.EmailOtpHash
            ) ||
            user.EmailOtpExpiresAt == null
        )
        {
            return null;
        }


        // =================================================
        // ATTEMPTS CHECK
        // =================================================

        if (
            user.EmailOtpAttempts >= 5
        )
        {
            return null;
        }


        // =================================================
        // EXPIRY CHECK
        // =================================================

        if (
            DateTime.UtcNow >
            user.EmailOtpExpiresAt.Value
        )
        {
            return null;
        }


        // =================================================
        // VERIFY OTP
        // =================================================

        var isCorrect =
            _emailOtpService
                .VerifyOtp(
                    request.Otp,
                    user.EmailOtpHash
                );


        // =================================================
        // WRONG OTP
        // =================================================

        if (!isCorrect)
        {
            user.EmailOtpAttempts++;


            await _userRepository
                .UpdateAsync(
                    user
                );


            return null;
        }


        // =================================================
        // EMAIL VERIFIED
        // =================================================

        user.IsEmailVerified =
            true;


        // Used OTP remove pannuvom
        user.EmailOtpHash =
            null;


        user.EmailOtpExpiresAt =
            null;


        user.EmailOtpAttempts =
            0;


        await _userRepository
            .UpdateAsync(
                user
            );


        // =================================================
        // TOTP SETUP TOKEN
        // =================================================
        //
        // Idhu final login JWT illa.
        //
        // 10 minute temporary setup token.
        //
        var setupToken =
            _registrationTokenService
                .GenerateTotpSetupToken(
                    user
                );


        return new EmailVerificationResponseDto
        {
            EmailVerified =
                true,

            SetupToken =
                setupToken,

            Message =
                "Email verified successfully. Continue authenticator setup."
        };
    }


    // =====================================================
    // RESEND EMAIL OTP
    // =====================================================

    public async Task<bool> ResendEmailOtpAsync(
        ResendEmailOtpRequestDto request)
    {
        var email =
            request.Email
                .Trim()
                .ToLowerInvariant();


        var user =
            await _userRepository
                .GetByEmailAsync(
                    email
                );


        if (user == null)
        {
            return false;
        }


        // Already verified account-ku
        // OTP resend panna thevai illa.
        if (user.IsEmailVerified)
        {
            return false;
        }


        // =================================================
        // 60 SECOND COOLDOWN
        // =================================================

        if (
            user.EmailOtpLastSentAt
                .HasValue
        )
        {
            var nextAllowed =
                user.EmailOtpLastSentAt
                    .Value
                    .AddSeconds(60);


            if (
                DateTime.UtcNow <
                nextAllowed
            )
            {
                return false;
            }
        }


        // =================================================
        // NEW OTP
        // =================================================

        var otp =
            _emailOtpService
                .GenerateOtp();


        user.EmailOtpHash =
            _emailOtpService
                .HashOtp(
                    otp
                );


        user.EmailOtpExpiresAt =
            DateTime.UtcNow
                .AddMinutes(5);


        user.EmailOtpAttempts =
            0;


        user.EmailOtpLastSentAt =
            DateTime.UtcNow;


        await _userRepository
            .UpdateAsync(
                user
            );


        // =================================================
        // EMAIL SEND
        // =================================================

        await _emailService
            .SendVerificationOtpAsync(
                user.Email,
                user.FullName,
                otp
            );


        return true;
    }


    // =====================================================
    // SETUP TOTP / GOOGLE AUTHENTICATOR
    // =====================================================
    //
    // INPUT:
    // SetupToken
    //
    // FLOW:
    //
    // Setup Token
    //      ↓
    // Validate
    //      ↓
    // User Find
    //      ↓
    // Email Verified Check
    //      ↓
    // TOTP Secret Generate
    //      ↓
    // Secret Encrypt
    //      ↓
    // DB Save
    //      ↓
    // QR Generate
    //
    public async Task<TotpSetupResponseDto?>
        SetupTotpAsync(
            SetupTotpRequestDto request)
    {
        // =================================================
        // VALIDATE TEMPORARY TOKEN
        // =================================================

        var userId =
            _registrationTokenService
                .ValidateTotpSetupToken(
                    request.SetupToken
                );


        if (userId == null)
        {
            return null;
        }


        // =================================================
        // USER FIND
        // =================================================

        var user =
            await _userRepository
                .GetByIdAsync(
                    userId.Value
                );


        if (user == null)
        {
            return null;
        }


        // Email verification mandatory
        if (!user.IsEmailVerified)
        {
            return null;
        }


        // Already TOTP enabled-na
        // new secret overwrite panna koodathu.
        if (user.IsTotpEnabled)
        {
            return null;
        }


        // =================================================
        // GENERATE TOTP SECRET
        // =================================================

        var secret =
            _totpService
                .GenerateSecret();


        // =================================================
        // ENCRYPT SECRET
        // =================================================
        //
        // Plain TOTP secret DB-la save panna maatom.
        //
        var encryptedSecret =
            _totpSecretProtector
                .Encrypt(
                    secret
                );


        user.TotpSecretEncrypted =
            encryptedSecret;


        user.IsTotpEnabled =
            false;


        await _userRepository
            .UpdateAsync(
                user
            );


        // =================================================
        // OTPAUTH URI
        // =================================================

        var otpAuthUri =
            _totpService
                .GenerateOtpAuthUri(
                    secret,
                    user.Email
                );


        // =================================================
        // QR CODE
        // =================================================

        var qrCode =
            _totpService
                .GenerateQrCodeDataUrl(
                    otpAuthUri
                );


        return new TotpSetupResponseDto
        {
            // Setup screen-la mattum user-ku show
            SecretKey =
                secret,

            OtpAuthUri =
                otpAuthUri,

            QrCodeDataUrl =
                qrCode
        };
    }


    // =====================================================
    // VERIFY INITIAL TOTP SETUP
    // =====================================================
    //
    // INPUT:
    //
    // SetupToken
    // 6 Digit Authenticator Code
    //
    // SUCCESS:
    //
    // IsTotpEnabled = true
    //
    public async Task<bool> VerifyTotpSetupAsync(
        VerifyTotpSetupRequestDto request)
    {
        // =================================================
        // VALIDATE SETUP TOKEN
        // =================================================

        var userId =
            _registrationTokenService
                .ValidateTotpSetupToken(
                    request.SetupToken
                );


        if (userId == null)
        {
            return false;
        }


        // =================================================
        // GET USER
        // =================================================

        var user =
            await _userRepository
                .GetByIdAsync(
                    userId.Value
                );


        if (user == null)
        {
            return false;
        }


        if (!user.IsEmailVerified)
        {
            return false;
        }


        // Already enabled-na success
        if (user.IsTotpEnabled)
        {
            return true;
        }


        // Secret missing
        if (
            string.IsNullOrWhiteSpace(
                user.TotpSecretEncrypted
            )
        )
        {
            return false;
        }


        // =================================================
        // DECRYPT TOTP SECRET
        // =================================================

        string secret;


        try
        {
            secret =
                _totpSecretProtector
                    .Decrypt(
                        user.TotpSecretEncrypted
                    );
        }
        catch
        {
            return false;
        }


        // =================================================
        // VERIFY 6 DIGIT TOTP
        // =================================================

        var valid =
            _totpService
                .VerifyCode(
                    secret,
                    request.Code
                );


        if (!valid)
        {
            return false;
        }


        // =================================================
        // TOTP ENABLED
        // =================================================

        user.IsTotpEnabled =
            true;


        await _userRepository
            .UpdateAsync(
                user
            );


        return true;
    }


    // =====================================================
    // STEP 7
    // LOGIN STEP 1
    // =====================================================
    //
    // INPUT:
    // Email + Password
    //
    // ADMIN:
    //
    // Password correct
    //      ↓
    // Direct Final JWT
    //
    //
    // CLIENT:
    //
    // Email + Password
    //      ↓
    // Email Verified?
    //      ↓
    // TOTP Enabled?
    //      ↓
    // 5-minute MFA Challenge Token
    //
    // IMPORTANT:
    //
    // Client-ku password verify panna odane
    // FINAL JWT return panna maatom.
    //
    public async Task<MfaChallengeResponseDto?> LoginAsync(
        LoginRequestDto request)
    {
        var email =
            request.Email
                .Trim()
                .ToLowerInvariant();


        // =================================================
        // USER FIND
        // =================================================

        var user =
            await _userRepository
                .GetByEmailAsync(
                    email
                );


        if (user == null)
        {
            return null;
        }


        // =================================================
        // ACCOUNT ACTIVE CHECK
        // =================================================

        if (!user.IsActive)
        {
            return null;
        }


        // =================================================
        // PASSWORD VERIFY
        // =================================================

        var passwordCorrect =
            _passwordHasher
                .VerifyPassword(
                    user,
                    user.PasswordHash,
                    request.Password
                );


        if (!passwordCorrect)
        {
            return null;
        }


        // =================================================
        // ROLE CHECK
        // =================================================

        var isAdmin =
            user.Role.Equals(
                "Admin",
                StringComparison.OrdinalIgnoreCase
            );


        // =================================================
        // ADMIN LOGIN
        // =================================================
        //
        // Current Admin login flow preserve pannrom.
        //
        // Admin-ku ippo TOTP compulsory illa.
        //
        if (isAdmin)
        {
            var adminToken =
                _jwtTokenGenerator
                    .GenerateToken(
                        user
                    );


            return new MfaChallengeResponseDto
            {
                RequiresTotp =
                    false,

                ChallengeToken =
                    null,

                UserId =
                    user.Id,

                FullName =
                    user.FullName,

                Email =
                    user.Email,

                Role =
                    user.Role,

                Token =
                    adminToken,

                Message =
                    "Admin login successful."
            };
        }


        // =================================================
        // CLIENT EMAIL VERIFICATION
        // =================================================
        //
        // Email OTP verification complete aagala na
        // login allow panna maatom.
        //
        if (!user.IsEmailVerified)
        {
            return null;
        }


        // =================================================
        // CLIENT TOTP CHECK
        // =================================================
        //
        // Google Authenticator setup complete aagala na
        // login allow panna maatom.
        //
        if (
            !user.IsTotpEnabled ||
            string.IsNullOrWhiteSpace(
                user.TotpSecretEncrypted
            )
        )
        {
            return null;
        }


        // =================================================
        // CREATE MFA CHALLENGE TOKEN
        // =================================================
        //
        // 5 minutes valid.
        //
        // Idhu final access JWT illa.
        //
        var challengeToken =
            _mfaChallengeTokenService
                .GenerateChallengeToken(
                    user
                );


        // =================================================
        // RETURN MFA CHALLENGE
        // =================================================
        //
        // Notice:
        //
        // Token = null
        //
        // TOTP verify panna piragu dhaan
        // actual JWT varum.
        //
        return new MfaChallengeResponseDto
        {
            RequiresTotp =
                true,

            ChallengeToken =
                challengeToken,

            UserId =
                user.Id,

            FullName =
                user.FullName,

            Email =
                user.Email,

            Role =
                user.Role,

            Token =
                null,

            Message =
                "Password verified. Enter your authenticator code."
        };
    }


    // =====================================================
    // STEP 8
    // LOGIN STEP 2 - VERIFY TOTP
    // =====================================================
    //
    // INPUT:
    //
    // ChallengeToken
    // Google Authenticator 6 Digit Code
    //
    //
    // FLOW:
    //
    // Challenge Token
    //       ↓
    // Validate Token
    //       ↓
    // User Find
    //       ↓
    // Active?
    //       ↓
    // Email Verified?
    //       ↓
    // TOTP Enabled?
    //       ↓
    // Encrypted Secret Decrypt
    //       ↓
    // Google Authenticator Code Verify
    //       ↓
    // FINAL JWT
    //
    // OUTPUT:
    //
    // LoginResponseDto with final JWT
    //
    public async Task<LoginResponseDto?> VerifyLoginTotpAsync(
        VerifyLoginTotpRequestDto request)
    {
        // =================================================
        // CHALLENGE TOKEN VALIDATE
        // =================================================

        var userId =
            _mfaChallengeTokenService
                .ValidateChallengeToken(
                    request.ChallengeToken
                );


        // Token invalid / expired
        if (userId == null)
        {
            return null;
        }


        // =================================================
        // USER FIND
        // =================================================

        var user =
            await _userRepository
                .GetByIdAsync(
                    userId.Value
                );


        if (user == null)
        {
            return null;
        }


        // =================================================
        // ACTIVE ACCOUNT CHECK
        // =================================================

        if (!user.IsActive)
        {
            return null;
        }


        // =================================================
        // EMAIL VERIFIED CHECK
        // =================================================

        if (!user.IsEmailVerified)
        {
            return null;
        }


        // =================================================
        // TOTP ENABLED CHECK
        // =================================================

        if (
            !user.IsTotpEnabled ||
            string.IsNullOrWhiteSpace(
                user.TotpSecretEncrypted
            )
        )
        {
            return null;
        }


        // =================================================
        // DECRYPT TOTP SECRET
        // =================================================

        string secret;


        try
        {
            secret =
                _totpSecretProtector
                    .Decrypt(
                        user.TotpSecretEncrypted
                    );
        }
        catch
        {
            // Secret decrypt panna mudiyala
            return null;
        }


        // =================================================
        // VERIFY GOOGLE AUTHENTICATOR CODE
        // =================================================

        var valid =
            _totpService
                .VerifyCode(
                    secret,
                    request.Code
                );


        if (!valid)
        {
            return null;
        }


        // =================================================
        // FINAL JWT GENERATE
        // =================================================
        //
        // Password verification already complete.
        //
        // Ippo TOTP-um success.
        //
        // Rendu security checks success aana piragu
        // dhaan final JWT generate pannrom.
        //
        var token =
            _jwtTokenGenerator
                .GenerateToken(
                    user
                );


        // =================================================
        // FINAL LOGIN RESPONSE
        // =================================================

        return new LoginResponseDto
        {
            UserId =
                user.Id,

            FullName =
                user.FullName,

            Email =
                user.Email,

            Role =
                user.Role,

            Token =
                token
        };
    }
}