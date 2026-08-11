import { api } from './apiClient.js'; export const searchApi = { search: q => api(`/search?keyword=${encodeURIComponent(q)}`) };
