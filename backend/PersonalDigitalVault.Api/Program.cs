using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using PersonalDigitalVault.Api.Data;
using PersonalDigitalVault.Api.Interfaces.Repositories;
using PersonalDigitalVault.Api.Interfaces.Services;
using PersonalDigitalVault.Api.Middleware;
using PersonalDigitalVault.Api.Repositories;
using PersonalDigitalVault.Api.Security;
using PersonalDigitalVault.Api.Services;
using PersonalDigitalVault.Api.Storage;

using System.Text;


// =========================================================
// APPLICATION BUILDER
// =========================================================

var builder =
    WebApplication.CreateBuilder(args);


// =========================================================
// CONTROLLERS
// =========================================================

builder.Services.AddControllers();


// HttpContext access panna use pannuvom
builder.Services.AddHttpContextAccessor();


// =========================================================
// DATABASE
// =========================================================
//
// SQL Server connection
//

builder.Services.AddDbContext<AppDbContext>(
    options =>
        options.UseSqlServer(
            builder.Configuration
                .GetConnectionString(
                    "DefaultConnection"
                )
        )
);


// =========================================================
// REPOSITORIES
// =========================================================

// User Repository
builder.Services.AddScoped<
    IUserRepository,
    UserRepository>();


// Folder Repository
builder.Services.AddScoped<
    IFolderRepository,
    FolderRepository>();


// Document Repository
builder.Services.AddScoped<
    IDocumentRepository,
    DocumentRepository>();


// Credential Repository
builder.Services.AddScoped<
    ICredentialRepository,
    CredentialRepository>();


// IMPORTANT:
// IUserRepository duplicate registration
// inga second time add panna thevai illa.


// =========================================================
// APPLICATION SERVICES
// =========================================================

// Authentication
builder.Services.AddScoped<
    IAuthService,
    AuthService>();


// Profile
builder.Services.AddScoped<
    IProfileService,
    ProfileService>();


// Folder
builder.Services.AddScoped<
    IFolderService,
    FolderService>();


// Document
builder.Services.AddScoped<
    IDocumentService,
    DocumentService>();


// Credential
builder.Services.AddScoped<
    ICredentialService,
    CredentialService>();


// Search
builder.Services.AddScoped<
    ISearchService,
    SearchService>();


// Admin
builder.Services.AddScoped<
    IAdminService,
    AdminService>();


// Email SMTP Service
builder.Services.AddScoped<
    IEmailService,
    EmailService>();


// =========================================================
// EMAIL OTP SECURITY
// =========================================================
//
// Email OTP:
//
// Generate
// Hash
// Verify
//

builder.Services.AddScoped<EmailOtpService>();



// =========================================================
// STEP 9 - TOTP / MFA SERVICES
// =========================================================
//
// Email verification mudinja piragu
// temporary TOTP setup token create/validate pannum.
//

builder.Services.AddScoped<
    RegistrationTokenService>();


// Google Authenticator compatible
// TOTP secret + code verification + QR generation
builder.Services.AddScoped<
    TotpService>();


// TOTP secret-a AES-GCM use panni
// encrypt/decrypt pannum.
builder.Services.AddScoped<
    TotpSecretProtector>();


// Client login MFA challenge token
builder.Services.AddScoped<
    MfaChallengeTokenService>();

// =========================================================
// EXISTING SECURITY / STORAGE HELPERS
// =========================================================

// Password hash / verify
builder.Services.AddSingleton<
    PasswordHasher>();


// Final JWT token generate pannum
builder.Services.AddSingleton<
    JwtTokenGenerator>();


// Credential / sensitive data AES encryption
builder.Services.AddSingleton<
    AesEncryption>();


// Uploaded file SHA-256 hash
builder.Services.AddSingleton<
    FileHashService>();


// Current logged-in user details
builder.Services.AddScoped<
    CurrentUserService>();


// Protected file storage
builder.Services.AddSingleton<
    FileStorageService>();


// =========================================================
// ADMIN DATABASE SEEDER
// =========================================================
//
// Application start aagumbodhu
// default admin irukka check pannum.
//

builder.Services.AddScoped<
    DbSeeder>();


// =========================================================
// JWT AUTHENTICATION
// =========================================================

var jwtKey =
    builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "Jwt:Key is missing."
    );


builder.Services
    .AddAuthentication(
        JwtBearerDefaults
            .AuthenticationScheme
    )
    .AddJwtBearer(
        options =>
        {
            options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    // Issuer validate pannum
                    ValidateIssuer = true,

                    // Audience validate pannum
                    ValidateAudience = true,

                    // JWT expiry validate pannum
                    ValidateLifetime = true,

                    // Signature validate pannum
                    ValidateIssuerSigningKey = true,


                    ValidIssuer =
                        builder.Configuration[
                            "Jwt:Issuer"
                        ],


                    ValidAudience =
                        builder.Configuration[
                            "Jwt:Audience"
                        ],


                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8
                                .GetBytes(jwtKey)
                        ),


                    // Token expire aana
                    // extra time kudukka maatom
                    ClockSkew =
                        TimeSpan.Zero
                };
        }
    );


// =========================================================
// ROLE AUTHORIZATION
// =========================================================
//
// Example:
//
// [Authorize]
// [Authorize(Roles = "Admin")]
//

builder.Services.AddAuthorization();


// =========================================================
// BUILD APPLICATION
// =========================================================

var app =
    builder.Build();


// =========================================================
// CREATE DEFAULT ADMIN
// =========================================================
//
// Application start aagumbodhu
// DbSeeder run pannum.
//

using (
    var scope =
        app.Services.CreateScope()
)
{
    var seeder =
        scope.ServiceProvider
            .GetRequiredService<
                DbSeeder>();


    await seeder.SeedAsync();
}


// =========================================================
// GLOBAL ERROR HANDLING
// =========================================================

app.UseMiddleware<
    ExceptionMiddleware>();


// =========================================================
// HTTPS
// =========================================================

app.UseHttpsRedirection();


// =========================================================
// FRONTEND
// =========================================================
//
// wwwroot/index.html default file
//

app.UseDefaultFiles();


// HTML / CSS / JavaScript serve pannum
app.UseStaticFiles();


// =========================================================
// AUTHENTICATION
// =========================================================
//
// IMPORTANT:
// Authentication always Authorization-ku
// munnaadi varanum.
//

app.UseAuthentication();


// =========================================================
// AUTHORIZATION
// =========================================================

app.UseAuthorization();


// =========================================================
// API CONTROLLERS
// =========================================================

app.MapControllers();


// =========================================================
// RUN APPLICATION
// =========================================================

app.Run();