using System.Net;
using System.Net.Mail;
using PersonalDigitalVault.Api.Interfaces.Services;

namespace PersonalDigitalVault.Api.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(
        IConfiguration configuration)
    {
        _configuration = configuration;
    }


    // =====================================================
    // SEND EMAIL OTP
    // =====================================================
    //
    // INPUT:
    // email    -> User email
    // fullName -> User name
    // otp      -> 6 digit verification OTP
    //
    // OUTPUT:
    // Email send aagum
    //
    public async Task SendVerificationOtpAsync(
        string email,
        string fullName,
        string otp)
    {
        var host =
            _configuration[
                "Email:SmtpHost"
            ];

        var portText =
            _configuration[
                "Email:SmtpPort"
            ];

        var username =
            _configuration[
                "Email:Username"
            ];

        var password =
            _configuration[
                "Email:Password"
            ];

        var fromEmail =
            _configuration[
                "Email:FromEmail"
            ];

        var fromName =
            _configuration[
                "Email:FromName"
            ];


        if (
            string.IsNullOrWhiteSpace(host) ||
            string.IsNullOrWhiteSpace(portText) ||
            string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(fromEmail)
        )
        {
            throw new InvalidOperationException(
                "Email configuration is incomplete."
            );
        }


        var port =
            int.Parse(
                portText
            );


        using var smtp =
            new SmtpClient(
                host,
                port
            );


        smtp.EnableSsl = true;


        smtp.Credentials =
            new NetworkCredential(
                username,
                password
            );


        using var message =
            new MailMessage();


        message.From =
            new MailAddress(
                fromEmail,
                fromName ??
                "Personal Digital Vault"
            );


        message.To.Add(
            email
        );


        message.Subject =
            "Personal Digital Vault - Email Verification Code";


        message.IsBodyHtml =
            true;


        message.Body = $@"
            <html>
            <body style='font-family:Arial,sans-serif;'>

                <h2>
                    Personal Digital Vault
                </h2>

                <p>
                    Hi {WebUtility.HtmlEncode(fullName)},
                </p>

                <p>
                    Your email verification code is:
                </p>

                <div style='
                    font-size:30px;
                    font-weight:bold;
                    letter-spacing:8px;
                    margin:20px 0;
                '>
                    {WebUtility.HtmlEncode(otp)}
                </div>

                <p>
                    This code expires in
                    <strong>5 minutes</strong>.
                </p>

                <p>
                    If you did not request this code,
                    you can ignore this email.
                </p>

            </body>
            </html>
        ";


        await smtp.SendMailAsync(
            message
        );
    }
}