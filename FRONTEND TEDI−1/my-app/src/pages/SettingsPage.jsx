import React, { useState, useEffect } from 'react';
import api from '../services/api'; // Import the configured API instance

const SettingsPage = () => {
    const [user, setUser] = useState(null);
    const [newEmail, setNewEmail] = useState('');
    const [password, setPassword] = useState('');
    const [newPassword, setNewPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [error, setError] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchUser = async () => {
            const emailParam = localStorage.getItem('email'); // Fetch email from localStorage
            const token = localStorage.getItem('token'); // Fetch token from localStorage

            if (!emailParam) {
                setError('No email found in local storage');
                setLoading(false);
                return;
            }

            if (!token) {
                setError('No token found in local storage');
                setLoading(false);
                return;
            }

            try {
                const response = await api.get(`/api/User/email/${emailParam}`, {
                    headers: {
                        Authorization: `Bearer ${token}`
                    }
                });
                setUser(response.data);
                setLoading(false);
            } catch (err) {
                console.error('Error fetching user data:', err);
                setError('Failed to fetch user data');
                setLoading(false);
            }
        };

        fetchUser();
    }, []);

    const handleEmailChange = async (e) => {
        e.preventDefault();
        setError(null); // Clear previous error
        const token = localStorage.getItem('token'); // Fetch token from localStorage

        if (!token) {
            setError('No token found in local storage');
            return;
        }

        const updatedUser = { ...user, email: newEmail };

        try {
            const response = await api.post(`/api/User/${user.id}`, updatedUser, {
                headers: {
                    Authorization: `Bearer ${token}`
                }
            });

            alert(response.data.message || 'Email changed successfully!');
            localStorage.setItem('email', newEmail);
            setNewEmail('');
        } catch (error) {
            console.error('Error changing email:', error.response ? error.response.data : error.message);
            setError(`Failed to change email: ${error.response?.data?.message || error.message}`);
        }
    };

    const handlePasswordChange = async (e) => {
        e.preventDefault();
        setError(null); // Clear previous error
        const token = localStorage.getItem('token'); // Fetch token from localStorage

        if (!token) {
            setError('No token found in local storage');
            return;
        }

        if (newPassword !== confirmPassword) {
            alert('New password and confirm password do not match');
            return;
        }

        try {
            const response = await api.post(`/api/User/${user.id}/change/password`, {
                oldPassword: password,
                newPassword: newPassword
            }, {
                headers: {
                    Authorization: `Bearer ${token}`
                }
            });

            alert(response.data.message || 'Password changed successfully!');
            setPassword('');
            setNewPassword('');
            setConfirmPassword('');
        } catch (error) {
            console.error('Error changing password:', error.response ? error.response.data : error.message);
            setError(`Failed to change password: ${error.response?.data?.message || error.message}`);
        }
    };

    if (loading) return <div>Loading...</div>;
    if (error) return <div style={{ color: 'red' }}>{error}</div>;
    if (!user) return <div>User not found</div>;

    return (
        <div style={styles.container}>
            <h1 style={styles.title}>Settings Page</h1>

            {error && <div style={styles.error}>{error}</div>}

            <form onSubmit={handleEmailChange} style={styles.form}>
                <div style={styles.formGroup}>
                    <label style={styles.label}>
                        Current Email:
                        <input
                            type="email"
                            value={user.email}
                            disabled
                            style={styles.inputDisabled}
                        />
                    </label>
                </div>
                <div style={styles.formGroup}>
                    <label style={styles.label}>
                        New Email:
                        <input
                            type="email"
                            value={newEmail}
                            onChange={(e) => setNewEmail(e.target.value)}
                            required
                            style={styles.input}
                        />
                    </label>
                </div>
                <button type="submit" style={styles.button}>Change Email</button>
            </form>

            <form onSubmit={handlePasswordChange} style={styles.form}>
                <div style={styles.formGroup}>
                    <label style={styles.label}>
                        Current Password:
                        <input
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                            style={styles.input}
                        />
                    </label>
                </div>
                <div style={styles.formGroup}>
                    <label style={styles.label}>
                        New Password:
                        <input
                            type="password"
                            value={newPassword}
                            onChange={(e) => setNewPassword(e.target.value)}
                            required
                            style={styles.input}
                        />
                    </label>
                </div>
                <div style={styles.formGroup}>
                    <label style={styles.label}>
                        Confirm New Password:
                        <input
                            type="password"
                            value={confirmPassword}
                            onChange={(e) => setConfirmPassword(e.target.value)}
                            required
                            style={styles.input}
                        />
                    </label>
                </div>
                <button type="submit" style={styles.button}>Change Password</button>
            </form>
        </div>
    );
};

const styles = {
    container: {
        padding: '2rem',
        maxWidth: '600px',
        margin: '0 auto',
        backgroundColor: '#f5f5f5',
        borderRadius: '8px',
        boxShadow: '0 2px 4px rgba(0, 0, 0, 0.1)',
        fontFamily: 'Arial, sans-serif',
    },
    title: {
        textAlign: 'center',
        marginBottom: '1.5rem',
        color: '#0073b1',
    },
    form: {
        marginBottom: '2rem',
        padding: '1.5rem',
        border: '1px solid #ddd',
        borderRadius: '5px',
        display: 'flex',
        flexDirection: 'column', // Ensures that the elements stack vertically
    },
    formGroup: {
        marginBottom: '1.5rem', // Increased margin for better spacing
        display: 'flex',
        flexDirection: 'column', // Stack label and input vertically
    },
    label: {
        display: 'block',
        marginBottom: '0.5rem',
        fontWeight: 'bold',
        color: '#333',
    },
    input: {
        width: '100%',
        padding: '0.5rem',
        borderRadius: '4px',
        border: '1px solid #0073b1',
        boxSizing: 'border-box', // Ensure padding is included in total width
    },
    inputDisabled: {
        width: '100%',
        padding: '0.5rem',
        borderRadius: '4px',
        border: '1px solid #ccc',
        backgroundColor: '#f9f9f9',
        boxSizing: 'border-box',
    },
    button: {
        padding: '0.5rem 1rem',
        backgroundColor: '#0073b1',
        color: 'white',
        border: 'none',
        borderRadius: '4px',
        cursor: 'pointer',
        transition: 'background-color 0.3s ease',
        fontWeight: 'bold',
        marginTop: '1rem', // Added margin for button separation
    },
    error: {
        color: 'red',
        marginBottom: '1rem',
    },
};

export default SettingsPage;
