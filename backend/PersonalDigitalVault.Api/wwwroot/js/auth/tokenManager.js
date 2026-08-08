const TOKEN_KEY = 'pdv_token';

export const tokenManager =
{
    // JWT token get pannum
    get() {
        return localStorage.getItem(TOKEN_KEY);
    },

    // Login successful aana new token save pannum
    set(token) {
        // Previous token irundha first remove pannrom
        localStorage.removeItem(TOKEN_KEY);

        // New token mattum save pannrom
        localStorage.setItem(
            TOKEN_KEY,
            token
        );
    },

    // Logout pannumbodhu authentication related
    // localStorage values ellam remove pannum
    clear() {
        localStorage.removeItem(TOKEN_KEY);

        // Future-la indha values store pannina
        // avayum clean aagum
        localStorage.removeItem('pdv_role');
        localStorage.removeItem('pdv_user');
        localStorage.removeItem('pdv_userId');
        localStorage.removeItem('pdv_email');
    }
};