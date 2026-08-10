import { api } from './apiClient.js';

export const adminApi =
{
    // Admin dashboard statistics
    dashboard: () =>
        api('/admin/dashboard'),

    // All users list
    users: () =>
        api('/admin/users'),

    // Enable / Disable user
    setStatus: (id, isActive) =>
        api(
            `/admin/users/${id}/status`,
            {
                method: 'PUT',

                body: JSON.stringify({
                    isActive: isActive
                })
            })
};