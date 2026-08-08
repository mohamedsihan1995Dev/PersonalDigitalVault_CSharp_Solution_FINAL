import { API_BASE_URL } from '../config.js';
import { tokenManager } from '../auth/tokenManager.js';

export async function api(path, options = {}) {
    const headers = new Headers(options.headers || {});
    const token = tokenManager.get();

    if (token) {
        headers.set('Authorization', `Bearer ${token}`);
    }

    if (options.body && !(options.body instanceof FormData)) {
        headers.set('Content-Type', 'application/json');
    }

    let response;

    try {
        response = await fetch(`${API_BASE_URL}${path}`, {
            ...options,
            headers
        });
    } catch (error) {
        console.error('API network error:', error);
        throw new Error('Server-ai connect panna mudiyala. API running-a irukka check pannunga.');
    }

    if (response.status === 204) {
        return null;
    }

    // Response JSON / text edhu vandhaalum safely read pannum.
    const rawText = await response.text();
    let data = null;

    if (rawText) {
        try {
            data = JSON.parse(rawText);
        } catch {
            data = rawText;
        }
    }

    if (!response.ok) {
        const backendMessage =
            (data && typeof data === 'object' && (data.message || data.title || data.detail)) ||
            (typeof data === 'string' && data.trim()) ||
            `Request failed: ${response.status}`;

        console.error('API error:', {
            url: `${API_BASE_URL}${path}`,
            status: response.status,
            response: data
        });

        throw new Error(backendMessage);
    }

    return data;
}
