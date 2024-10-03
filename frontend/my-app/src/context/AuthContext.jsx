import React, { createContext, useState, useEffect, useContext } from 'react';
import api from '../services/api'; // Import the real API

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
    const [user, setUser] = useState(null);

    useEffect(() => {
        const token = localStorage.getItem('token');
        const email = localStorage.getItem('email');
        const id = localStorage.getItem('id');

        if (token && email && id) {
            setUser({ email, id, token });
        } else {
            console.warn('No user data found in localStorage.');
        }
    }, []);

    const login = async (email, password) => {
        try {
            const response = await api.post('api/Auth/login', { email, password });

            const { Id, Role, Token } = response.data;

            if (Id && Role && Token) {
                setUser({ email, id: Id, role: Role, token: Token });
                localStorage.setItem('token', Token);
                localStorage.setItem('id', Id);
                localStorage.setItem('email', email);

                return { success: true, role: Role, id: Id };
            } else {
                console.error('Login response is missing data:', response.data);
                return { success: false, message: 'Invalid email or password' };
            }
        } catch (error) {
            console.error('Login failed', error);
            return { success: false, message: error.response ? error.response.data.message : 'Login failed' };
        }
    };

    const logout = () => {
        setUser(null);
        localStorage.removeItem('token');
        localStorage.removeItem('email');
        localStorage.removeItem('id');
    };

    return (
        <AuthContext.Provider value={{ user, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => {
    return useContext(AuthContext);
};
