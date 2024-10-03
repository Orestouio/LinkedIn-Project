// src/utils/auth.js
export const isAuthenticated = () => {
    // Logic to check if user is authenticated, e.g., checking a token in local storage
    return localStorage.getItem('token') !== null;
};

export const getToken = () => {
    return localStorage.getItem('token');
};

export const setToken = (token) => {
    localStorage.setItem('token', token);
};

export const removeToken = () => {
    localStorage.removeItem('token');
};
