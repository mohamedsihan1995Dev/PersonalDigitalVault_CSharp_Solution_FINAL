import { api } from './apiClient.js';

export const credentialApi = {
    all: () => api('/credentials'),

    get: (id) => api(`/credentials/${id}`),

    create: (data) =>
        api('/credentials', {
            method: 'POST',
            body: JSON.stringify(data),
        }),

    update: (id, data) =>
        api(`/credentials/${id}`, {
            method: 'PUT',
            body: JSON.stringify(data),
        }),

    remove: (id) =>
        api(`/credentials/${id}`, {
            method: 'DELETE',
        }),
};
