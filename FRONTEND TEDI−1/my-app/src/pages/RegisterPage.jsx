import React, { useState } from 'react';
import { useHistory } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import api from '../services/api';
import { Button } from '@mui/material';

const RegisterPage = () => {
    const history = useHistory();
    const { login } = useAuth();
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');
    const [phone, setPhone] = useState('');
    const [photo, setPhoto] = useState(null);

    const handleSubmit = async (event) => {
        event.preventDefault();

        if (password !== confirmPassword) {
            alert('Passwords do not match');
            return;
        }

        try {
            const registrationData = new FormData();
            registrationData.append('email', email);
            registrationData.append('password', password);
            registrationData.append('name', firstName);
            registrationData.append('surname', lastName);
            registrationData.append('phoneNumber', phone);
            registrationData.append('location', "default location");

            const response = await api.post('/api/Auth/register', registrationData);
            console.log('API Response:', response);

            if (response.data && response.status === 200) {
                const loginResult = await login(email, password);
                if (loginResult.success) {
                    console.log('Login success');
                    const { id } = loginResult;

                    // After successful login, upload the image if it exists
                    if (photo) {
                        const formData = new FormData();
                        formData.append('file', photo);
                        formData.append('ContentType', photo.type);
                        formData.append('FileName', photo.name);
                        const token = localStorage.getItem('token');

                        const uploadResponse = await api.post('/api/Image/upload', formData, {
                            headers: {
                                Authorization: `Bearer ${token}`,
                            }
                        });

                        if (uploadResponse.data && uploadResponse.status === 200) {
                            console.log('Image uploaded successfully:', uploadResponse.data.url);
                        } else {
                            alert('Image upload failed');
                        }
                    }

                    history.push(`/home/${id}`); // Redirect to home after login
                } else {
                    alert('Registration successful, but login failed.');
                }
                console.log('Registration successful');
            } else {
                alert('Registration failed');
                console.error('Registration failed:', response.data);
            }
        } catch (error) {
            if (error.response && error.response.status === 409) {
                alert('Email already in use. Please use a different email address.');
            } else {
                console.error('Registration error:', error);
                alert('Failed to register');
            }
        }
    };

    return (
        <div className="container">
            <h2>Register</h2>
            <form onSubmit={handleSubmit}>
                <div className="input-field">
                    <input
                        type="text"
                        value={firstName}
                        onChange={(e) => setFirstName(e.target.value)}
                        placeholder="First Name"
                        required
                    />
                </div>
                <div className="input-field">
                    <input
                        type="text"
                        value={lastName}
                        onChange={(e) => setLastName(e.target.value)}
                        placeholder="Last Name"
                        required
                    />
                </div>
                <div className="input-field">
                    <input
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        placeholder="Email"
                        required
                    />
                </div>
                <div className="input-field">
                    <input
                        type="text"
                        value={phone}
                        onChange={(e) => setPhone(e.target.value)}
                        placeholder="Contact Phone"
                        required
                    />
                </div>
                <div className="input-field">
                    <input
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        placeholder="Password"
                        required
                    />
                </div>
                <div className="input-field">
                    <input
                        type="password"
                        value={confirmPassword}
                        onChange={(e) => setConfirmPassword(e.target.value)}
                        placeholder="Confirm Password"
                        required
                    />
                </div>
                
                <button type="submit" className="btn">Register</button>
            </form>
        </div>
    );
};

export default RegisterPage;
