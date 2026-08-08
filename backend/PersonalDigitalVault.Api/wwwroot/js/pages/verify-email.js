import {
    authApi
} from '../api/authApi.js';


// =========================================================
// EMAIL
// =========================================================
//
// Register page-la sessionStorage-la save panna email.
//

const email =
    sessionStorage.getItem(
        'pdv_registration_email'
    );


// Email illaina registration flow start pannala.
if (!email) {

    window.location.replace(
        '/html/register.html'
    );
}


// =========================================================
// HTML ELEMENTS
// =========================================================

const form =
    document.getElementById(
        'verifyEmailForm'
    );

const emailDisplay =
    document.getElementById(
        'emailDisplay'
    );

const message =
    document.getElementById(
        'message'
    );

const resendButton =
    document.getElementById(
        'resendButton'
    );

const submitButton =
    form?.querySelector(
        'button[type="submit"]'
    );


// Email screen-la show pannuvom.
if (emailDisplay) {
    emailDisplay.textContent =
        email;
}


// =========================================================
// VERIFY EMAIL
// =========================================================

form?.addEventListener(
    'submit',
    async event => {

        event.preventDefault();

        message.textContent =
            '';


        const otp =
            form.otp
                .value
                .trim();


        // 6 digit validation
        if (!/^\d{6}$/.test(otp)) {

            message.textContent =
                'Enter a valid 6-digit verification code.';

            return;
        }


        submitButton.disabled =
            true;

        submitButton.textContent =
            'Verifying...';


        try {

            // =============================================
            // VERIFY EMAIL BACKEND
            // =============================================

            const result =
                await authApi.verifyEmail(
                    {
                        email:
                            email,

                        otp:
                            otp
                    }
                );


            if (
                !result ||
                !result.setupToken
            ) {

                throw new Error(
                    'TOTP setup token was not returned.'
                );
            }


            // =============================================
            // SAVE TEMPORARY SETUP TOKEN
            // =============================================

            sessionStorage.setItem(
                'pdv_totp_setup_token',
                result.setupToken
            );


            // Old QR cache clear
            sessionStorage.removeItem(
                'pdv_totp_setup_data'
            );


            // =============================================
            // NEXT PAGE
            // =============================================

            window.location.href =
                '/html/setup-totp.html';

        }
        catch (error) {

            console.error(
                'Email verification failed:',
                error
            );


            message.textContent =
                error.message ||
                'Email verification failed.';
        }
        finally {

            submitButton.disabled =
                false;

            submitButton.textContent =
                'Verify email';
        }
    }
);


// =========================================================
// RESEND OTP
// =========================================================

resendButton?.addEventListener(
    'click',
    async () => {

        message.textContent =
            '';


        resendButton.disabled =
            true;

        resendButton.textContent =
            'Sending...';


        try {

            await authApi.resendEmailOtp(
                {
                    email:
                        email
                }
            );


            message.className =
                'success';

            message.textContent =
                'New verification code sent successfully.';

        }
        catch (error) {

            message.className =
                'error';

            message.textContent =
                error.message ||
                'Unable to resend verification code.';
        }
        finally {

            resendButton.disabled =
                false;

            resendButton.textContent =
                'Resend code';
        }
    }
);