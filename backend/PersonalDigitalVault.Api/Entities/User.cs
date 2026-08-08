//namespace PersonalDigitalVault.Api.Entities;

//public class User
//{
//    public int Id { get; set; }
//    public string FullName { get; set; } = string.Empty;
//    public string Email { get; set; } = string.Empty;
//    public string PasswordHash { get; set; } = string.Empty;
//    public string Role { get; set; } = "User";
//    public bool IsActive { get; set; } = true;
//    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
//    public ICollection<Folder> Folders { get; set; } = new List<Folder>();
//    public ICollection<Document> Documents { get; set; } = new List<Document>();
//    public ICollection<Credential> Credentials { get; set; } = new List<Credential>();

//}

namespace PersonalDigitalVault.Api.Entities;

public class User
{
    // =====================================================
    // BASIC USER DETAILS
    // =====================================================

    // Database primary key
    public int Id { get; set; }


    // User full name
    public string FullName { get; set; }
        = string.Empty;


    // Login / verification email
    public string Email { get; set; }
        = string.Empty;


    // Plain password store panna maatom.
    // Password hash mattum inga save aagum.
    public string PasswordHash { get; set; }
        = string.Empty;


    // User / Admin
    public string Role { get; set; }
        = "User";


    // Admin user account-a
    // Enable / Disable panna use pannuvaar.
    public bool IsActive { get; set; }
        = true;


    // Account create panna date/time
    public DateTime CreatedAt { get; set; }
        = DateTime.UtcNow;



    // =====================================================
    // EMAIL VERIFICATION
    // =====================================================

    // false:
    // Email verification complete aagala.
    //
    // true:
    // Email OTP successfully verify aagiduchu.
    public bool IsEmailVerified { get; set; }
        = false;


    // Email-ku send panna OTP-oda
    // secure hash mattum database-la store pannuvom.
    //
    // Plain OTP database-la save panna maatom.
    public string? EmailOtpHash { get; set; }


    // OTP eppo expire aagum.
    //
    // Example:
    // OTP send time + 5 minutes
    public DateTime? EmailOtpExpiresAt { get; set; }


    // Wrong OTP attempts count panna use pannuvom.
    //
    // Example:
    // Maximum 5 attempts.
    public int EmailOtpAttempts { get; set; }
        = 0;


    // OTP resend spam avoid panna
    // last send time store pannuvom.
    public DateTime? EmailOtpLastSentAt { get; set; }



    // =====================================================
    // TOTP / TWO FACTOR AUTHENTICATION
    // =====================================================

    // Google Authenticator compatible
    // TOTP secret key.
    //
    // IMPORTANT:
    // Secret plaintext database-la save panna koodathu.
    //
    // Existing AesEncryption service use panni
    // encrypt pannitu save pannuvom.
    public string? TotpSecretEncrypted { get; set; }


    // false:
    // Authenticator setup complete aagala.
    //
    // true:
    // User first TOTP successfully verify pannitaar.
    public bool IsTotpEnabled { get; set; }
        = false;



    // =====================================================
    // NAVIGATION PROPERTIES
    // =====================================================

    // User folders
    public ICollection<Folder> Folders { get; set; }
        = new List<Folder>();


    // User uploaded documents
    public ICollection<Document> Documents { get; set; }
        = new List<Document>();


    // User encrypted credentials
    public ICollection<Credential> Credentials { get; set; }
        = new List<Credential>();
}