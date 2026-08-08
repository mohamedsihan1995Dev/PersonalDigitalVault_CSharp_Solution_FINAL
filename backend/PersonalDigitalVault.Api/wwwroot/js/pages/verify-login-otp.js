import {
    authApi
} from '../api/authApi.js';

import {
    tokenManager
} from '../auth/tokenManager.js';


// =========================================================
// MFA CHALLENGE TOKEN
// =========================================================
//
// Login Email + Password successful aana
// backend return panna temporary token.
//

const challengeToken =
    sessionStorage.getItem(
        'pdv_mfa_challenge'
    );


// Challenge illaina login page-ku thirumba pogum.
if (!challengeToken) {

    window.location.replace(
        '/html/login.html'
    );
}


// =========================================================
// HTML
// =========================================================

const form =
    document.getElementById(
        'loginTotpForm'
    );

const message =
    document.getElementById(
        'message'
    );

const submitButton =
    form?.querySelector(
        'button[type="submit"]'
    );


// =========================================================
// LOGIN STEP 2
// =========================================================

form?.addEventListener(
    'submit',
    async event => {

        event.preventDefault();


        message.textContent =
            '';


        const code =
            form.code
                .value
                .trim();


        if (!/^\d{6}$/.test(code)) {

            message.textContent =
                'Enter a valid 6-digit authenticator code.';

            return;
        }


        submitButton.disabled =
            true;

        submitButton.textContent =
            'Verifying...';


        try {

            // =============================================
            // VERIFY TOTP
            // =============================================

            const result =
                await authApi.verifyLoginTotp(
                    {
                        challengeToken:
                            challengeToken,

                        code:
                            code
                    }
                );


            // =============================================
            // FINAL JWT CHECK
            // =============================================

            if (
                !result ||
                !result.token
            ) {

                throw new Error(
                    'Final JWT token was not returned.'
                );
            }


            // =============================================
            // FINAL JWT SAVE
            // =============================================

            tokenManager.set(
                result.token
            );


            // Temporary MFA token delete
            sessionStorage.removeItem(
                'pdv_mfa_challenge'
            );


            // =============================================
            // REDIRECT
            // =============================================

            if (
                String(result.role)
                    .toLowerCase()
                === 'admin'
            ) {

                window.location.href =
                    '/html/admin.html';

                return;
            }


            window.location.href =
                '/html/dashboard.html';

        }
        catch (error) {

            console.error(
                'Authenticator login failed:',
                error
            );


            message.textContent =
                error.message ||
                'Authenticator verification failed.';
        }
        finally {

            submitButton.disabled =
                false;

            submitButton.textContent =
                'Verify and enter vault';
        }
    }
);