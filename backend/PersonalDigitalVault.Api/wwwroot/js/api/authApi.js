import {
    api
} from './apiClient.js';


// =========================================================
// AUTH API
// =========================================================
//
// Frontend authentication related ella backend endpoints-um
// indha object moolama call pannuvom.
//

export const authApi = {

    // =====================================================
    // REGISTER
    // =====================================================
    //
    // INPUT:
    // fullName
    // email
    // password
    //
    // OUTPUT:
    // userId
    // email
    // requiresEmailVerification
    //
    register(data) {
        return api(
            '/auth/register',
            {
                method: 'POST',
                body: JSON.stringify(data)
            }
        );
    },


    // =====================================================
    // VERIFY EMAIL OTP
    // =====================================================
    //
    // INPUT:
    // email
    // otp
    //
    // OUTPUT:
    // setupToken
    //
    verifyEmail(data) {
        return api(
            '/auth/verify-email',
            {
                method: 'POST',
                body: JSON.stringify(data)
            }
        );
    },


    // =====================================================
    // RESEND EMAIL OTP
    // =====================================================

    resendEmailOtp(data) {
        return api(
            '/auth/resend-email-otp',
            {
                method: 'POST',
                body: JSON.stringify(data)
            }
        );
    },


    // =====================================================
    // SETUP TOTP
    // =====================================================
    //
    // INPUT:
    // setupToken
    //
    // OUTPUT:
    // secretKey
    // otpAuthUri
    // qrCodeDataUrl
    //
    setupTotp(data) {
        return api(
            '/auth/setup-totp',
            {
                method: 'POST',
                body: JSON.stringify(data)
            }
        );
    },


    // =====================================================
    // VERIFY INITIAL TOTP SETUP
    // =====================================================

    verifyTotpSetup(data) {
        return api(
            '/auth/verify-totp-setup',
            {
                method: 'POST',
                body: JSON.stringify(data)
            }
        );
    },


    // =====================================================
    // LOGIN STEP 1
    // =====================================================
    //
    // User:
    // Email + Password
    //      ↓
    // MFA Challenge Token
    //
    // Admin:
    // Email + Password
    //      ↓
    // Final JWT
    //
    login(data) {
        return api(
            '/auth/login',
            {
                method: 'POST',
                body: JSON.stringify(data)
            }
        );
    },


    // =====================================================
    // LOGIN STEP 2
    // =====================================================
    //
    // ChallengeToken + TOTP
    //      ↓
    // Final JWT
    //
    verifyLoginTotp(data) {
        return api(
            '/auth/verify-login-totp',
            {
                method: 'POST',
                body: JSON.stringify(data)
            }
        );
    },


    // =====================================================
    // LOGOUT
    // =====================================================

    logout() {
        return api(
            '/auth/logout',
            {
                method: 'POST'
            }
        );
    }
};