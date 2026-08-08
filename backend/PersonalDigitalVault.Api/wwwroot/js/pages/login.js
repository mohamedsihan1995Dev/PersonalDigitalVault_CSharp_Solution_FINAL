import {
    authApi
} from '../api/authApi.js';

import {
    tokenManager
} from '../auth/tokenManager.js';


// =========================================================
// HTML ELEMENTS
// =========================================================

const form =
    document.querySelector(
        'form'
    );

const message =
    document.getElementById(
        'message'
    );

const submitButton =
    form?.querySelector(
        'button'
    );


// =========================================================
// OLD LOGIN DATA CLEAR
// =========================================================

// Login page open aagumbodhu
// previous temporary challenge remove pannuvom.
sessionStorage.removeItem(
    'pdv_mfa_challenge'
);


// Authenticator setup success message
const parameters =
    new URLSearchParams(
        window.location.search
    );


if (
    parameters.get('setup') ===
    'success'
) {

    message.className =
        'success';

    message.textContent =
        'Two-factor authentication enabled. Please sign in.';
}


// =========================================================
// LOGIN STEP 1
// =========================================================

form?.addEventListener(
    'submit',
    async event => {

        event.preventDefault();


        message.className =
            'error';

        message.textContent =
            '';


        submitButton.disabled =
            true;

        submitButton.textContent =
            'Signing in...';


        try {

            const result =
                await authApi.login(
                    {
                        email:
                            form.email
                                .value
                                .trim(),

                        password:
                            form.password
                                .value
                    }
                );


            // =============================================
            // CLIENT LOGIN
            // =============================================
            //
            // Password successful
            // Final JWT innum illa.
            //
            // MFA challenge irukkum.
            //

            if (
                result.requiresTotp === true
            ) {

                if (
                    !result.challengeToken
                ) {

                    throw new Error(
                        'MFA challenge token missing.'
                    );
                }


                sessionStorage.setItem(
                    'pdv_mfa_challenge',
                    result.challengeToken
                );


                window.location.href =
                    '/html/verify-login-otp.html';

                return;
            }


            // =============================================
            // ADMIN LOGIN
            // =============================================
            //
            // Admin-ku current flow direct JWT.
            //

            if (!result.token) {

                throw new Error(
                    'Login token was not returned.'
                );
            }


            tokenManager.set(
                result.token
            );


            // Admin role
            if (
                String(result.role)
                    .toLowerCase()
                === 'admin'
            ) {

                window.location.href =
                    '/html/admin.html';

                return;
            }


            // Normal fallback
            window.location.href =
                '/html/dashboard.html';

        }
        catch (error) {

            console.error(
                'Login failed:',
                error
            );


            message.textContent =
                error.message ||
                'Login failed.';
        }
        finally {

            submitButton.disabled =
                false;

            submitButton.textContent =
                'Login securely';
        }
    }
);