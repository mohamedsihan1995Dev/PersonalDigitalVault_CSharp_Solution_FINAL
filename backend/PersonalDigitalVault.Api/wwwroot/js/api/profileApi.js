import{api}from'./apiClient.js';export const profileApi={get:()=>api('/profile'),update:d=>api('/profile',{method:'PUT',body:JSON.stringify(d)})};
