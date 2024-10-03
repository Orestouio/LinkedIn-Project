import React, { useState } from 'react';
import { Link, useHistory, useLocation } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const Navbar = () => {
    const history = useHistory();
    const location = useLocation();
    const { user, login, logout } = useAuth();
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');

    const handleLogin = async (event) => {
        event.preventDefault();

        try {
            const loginResult = await login(email, password);
            console.log(loginResult);
            if (loginResult.success) {
                console.log('Login success');
                const { role, id } = loginResult;

                if (role.toLowerCase() === 'admin') {
                    history.push(`/admin/${id}`);
                } else {
                    history.push(`/home/${id}`);
                }
            } else {
                console.log('Login failed', loginResult.message);
                alert('Failed to login');
            }
        } catch (error) {
            console.error('Login error:', error);
            alert('Failed to login');
        }
    };

    const handleLogout = () => {
        logout();
        history.push('/'); // Redirect to the welcome page
    };

    return (
        <nav>
            {user ? (
                location.pathname.startsWith('/admin') ? (
                    <div>
                        <button onClick={handleLogout}>Logout</button>
                    </div>
                ) : (
                    <div>
                        <Link to={`/home/${user.id}`}>Home</Link>
                        <Link to={`/network/${user.id}`}>Network</Link>
                        <Link to={`/ads/${user.id}`}>Ads</Link>
                        <Link to={`/chat/${user.id}`}>Chats</Link>
                        <Link to={`/notifications/${user.id}`}>Notifications</Link>
                        <Link to={`/personal-info/${user.id}`}>Personal Information</Link>
                        <Link to={`/settings/${user.id}`}>Settings</Link>
                        <button onClick={handleLogout}>Logout</button>
                    </div>
                )
            ) : (
                location.pathname === '/' || location.pathname === '/register' ? (
                    <form onSubmit={handleLogin}>
                        <input
                            type="email"
                            placeholder="Email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            required
                        />
                        <input
                            type="password"
                            placeholder="Password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                        />
                        <button type="submit">Login</button>
                        {location.pathname !== '/register' && (
                            <Link to="/register">
                                <button type="button">Sign Up</button>
                            </Link>
                        )}
                    </form>
                ) : (
                    <div>
                        <Link to="/login">Login</Link>
                        <Link to="/register">Register</Link>
                    </div>
                )
            )}
        </nav>
    );
};

export default Navbar;
