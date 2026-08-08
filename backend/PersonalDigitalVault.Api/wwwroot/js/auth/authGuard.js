import {
    tokenManager
} from './tokenManager.js';


export function requireAuth() {
    const token =
        tokenManager.get();


    if (!token) {
        tokenManager.clear();

        window.location.replace(
            '/html/login.html'
        );

        return false;
    }


    return true;
}