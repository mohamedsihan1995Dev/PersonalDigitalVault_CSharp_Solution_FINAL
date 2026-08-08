import {
    authApi
} from '../api/authApi.js';


// =========================================================
// HTML ELEMENTS
// =========================================================

const form =
    document.querySelector('form');

const message =
    document.getElementById('message');

const submitButton =
    form?.querySelector('button');


// =========================================================
// REGISTER
// =========================================================

form?.addEventListener(
    'submit',
    async event => {

        event.preventDefault();

        message.textContent =
            '';


        // Button disable
        submitButton.disabled =
            true;

        submitButton.textContent =
            'Creating account...';


        try {

            // =============================================
            // BACKEND REGISTER API
            // =============================================

            const result =
                await authApi.register(
                    {
                        fullName:
                            form.fullName
                                .value
                                .trim(),

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
            // EMAIL SAVE TEMPORARILY
            // =============================================
            //
            // Email verification page-la use pannuvom.
            //
            // Final JWT illa.
            //

            sessionStorage.setItem(
                'pdv_registration_email',
                result.email
            );


            // Old setup data irundha clear pannuvom.
            sessionStorage.removeItem(
                'pdv_totp_setup_token'
            );

            sessionStorage.removeItem(
                'pdv_totp_setup_data'
            );


            // =============================================
            // EMAIL VERIFY PAGE
            // =============================================

            window.location.href =
                '/html/verify-email.html';

        }
        catch (error) {

            console.error(
                'Registration failed:',
                error
            );


            message.textContent =
                error.message ||
                'Registration failed.';
        }
        finally {

            submitButton.disabled =
                false;

            submitButton.textContent =
                'Create account';
        }
    }
);