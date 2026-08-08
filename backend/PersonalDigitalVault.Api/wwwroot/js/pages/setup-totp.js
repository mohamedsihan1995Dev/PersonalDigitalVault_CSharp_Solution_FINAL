import {
    authApi
} from '../api/authApi.js';


// =========================================================
// TEMPORARY SETUP TOKEN
// =========================================================

const setupToken =
    sessionStorage.getItem(
        'pdv_totp_setup_token'
    );


if (!setupToken) {

    window.location.replace(
        '/html/register.html'
    );
}


// =========================================================
// HTML ELEMENTS
// =========================================================

const qrContainer =
    document.getElementById(
        'qrContainer'
    );

const secretKeyElement =
    document.getElementById(
        'secretKey'
    );

const form =
    document.getElementById(
        'totpSetupForm'
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
// LOAD TOTP SETUP
// =========================================================

async function loadTotpSetup() {

    try {

        // Reload pannumbodhu new secret create aaga koodathu.
        // Already generated data sessionStorage-la irundha use pannuvom.
        const cached =
            sessionStorage.getItem(
                'pdv_totp_setup_data'
            );


        let result;


        if (cached) {

            result =
                JSON.parse(cached);

        }
        else {

            // =============================================
            // BACKEND TOTP SETUP
            // =============================================

            result =
                await authApi.setupTotp(
                    {
                        setupToken:
                            setupToken
                    }
                );


            sessionStorage.setItem(
                'pdv_totp_setup_data',
                JSON.stringify(result)
            );
        }


        // =============================================
        // QR IMAGE
        // =============================================

        if (!result.qrCodeDataUrl) {

            throw new Error(
                'QR code was not generated.'
            );
        }


        const image =
            document.createElement(
                'img'
            );


        image.src =
            result.qrCodeDataUrl;

        image.alt =
            'Authenticator QR Code';


        qrContainer.innerHTML =
            '';

        qrContainer.appendChild(
            image
        );


        // =============================================
        // MANUAL SECRET
        // =============================================

        secretKeyElement.textContent =
            result.secretKey ||
            'Unavailable';

    }
    catch (error) {

        console.error(
            'TOTP setup failed:',
            error
        );


        qrContainer.innerHTML =
            '<p class="error">Unable to load QR code.</p>';


        message.textContent =
            error.message ||
            'Authenticator setup failed.';
    }
}


// =========================================================
// VERIFY INITIAL TOTP
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
                'Enter the 6-digit code from your Authenticator app.';

            return;
        }


        submitButton.disabled =
            true;

        submitButton.textContent =
            'Verifying...';


        try {

            await authApi.verifyTotpSetup(
                {
                    setupToken:
                        setupToken,

                    code:
                        code
                }
            );


            // =============================================
            // SETUP COMPLETE
            // =============================================

            sessionStorage.removeItem(
                'pdv_totp_setup_token'
            );

            sessionStorage.removeItem(
                'pdv_totp_setup_data'
            );

            sessionStorage.removeItem(
                'pdv_registration_email'
            );


            // Login page-ku pogum
            window.location.href =
                '/html/login.html?setup=success';

        }
        catch (error) {

            console.error(
                'TOTP verification failed:',
                error
            );


            message.textContent =
                error.message ||
                'Invalid authenticator code.';
        }
        finally {

            submitButton.disabled =
                false;

            submitButton.textContent =
                'Enable two-factor authentication';
        }
    }
);


// Initial QR load
loadTotpSetup();