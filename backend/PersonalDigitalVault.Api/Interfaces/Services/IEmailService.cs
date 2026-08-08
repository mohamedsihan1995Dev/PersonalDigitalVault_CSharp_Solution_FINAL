namespace PersonalDigitalVault.Api.Interfaces.Services;

public interface IEmailService
{
    // User verification OTP email send pannum
    Task SendVerificationOtpAsync(
        string email,
        string fullName,
        string otp);
}