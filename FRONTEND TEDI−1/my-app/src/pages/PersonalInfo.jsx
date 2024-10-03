import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import api from '../services/api'; // Import the configured API instance

const PersonalInfoPage = () => {
    const { userId, searchedUserId } = useParams(); // Get both the logged-in user ID and the searched user ID from the URL
    const [user, setUser] = useState(null);
    const [editing, setEditing] = useState(false);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [isAdmin, setIsAdmin] = useState(false); // State to check if the user is an admin

    const loggedInUserId = localStorage.getItem('id'); // Get the logged-in user ID from localStorage
    const userRole = localStorage.getItem('role'); // Get the user role from localStorage

    useEffect(() => {
        // Check if the logged-in user is an admin
        setIsAdmin(userRole === 'admin');

        const fetchUser = async () => {
            const token = localStorage.getItem('token'); // Retrieve token from localStorage

            if (!token) {
                console.error('No token found, please log in again.');
                setError('No token found, please log in again.');
                return;
            }

            const userToFetchId = searchedUserId || userId; // Fetch the searched user or the logged-in user info
            console.log('Fetching user data for ID:', userToFetchId); // Debug: Check the user ID being fetched

            try {
                const response = await api.get(`/api/User/${userToFetchId}`, {
                    headers: {
                        Authorization: `Bearer ${token}`,  // Ensure the Bearer token is included in the request headers
                    },
                });

                console.log('User data received:', response.data); // Debug: Log the full response
                const userData = response.data.value; // Extract the 'value' from the response
                console.log('Extracted user data:', userData); // Debug: Log the extracted user data
                setUser(userData);
                setLoading(false);
            } catch (err) {
                console.error('Error fetching user data:', err.response ? err.response.data : err.message);
                setError('Failed to fetch user data');
                setLoading(false);
            }
        };

        fetchUser();
    }, [userId, searchedUserId, userRole]);

    // Check if the profile being viewed is the logged-in user's own profile
    const isOwnProfile = !searchedUserId && String(loggedInUserId) === String(userId);

    if (loading) return <div style={styles.loading}>Loading...</div>;
    if (error) return <div style={styles.error}>{error}</div>;
    if (!user) return <div style={styles.error}>User not found</div>;

    // Handle input change for hideableInfo fields
    const handleInputChange = (e) => {
        const { name, value } = e.target;
        if (name in user.hideableInfo) {
            setUser({
                ...user,
                hideableInfo: {
                    ...user.hideableInfo,
                    [name]: value,
                },
            });
        } else {
            setUser({ ...user, [name]: value });
        }
    };

    // Handle checkbox changes for hideableInfo public flags
    const handleCheckboxChange = (e) => {
        const { name, checked } = e.target;
        setUser({
            ...user,
            hideableInfo: {
                ...user.hideableInfo,
                [`${name}IsPublic`]: checked,
            },
        });
    };

    const saveChanges = async () => {
        const token = localStorage.getItem('token'); // Fetch the token from localStorage

        if (!token) {
            setError('No token found, please login again.');
            return;
        }

        try {
            console.log('Saving changes for user:', user); // Debug: Log the user data being saved

            const updatedUser = {
                ...user,
                hideableInfo: {
                    ...user.hideableInfo,
                    capabilities: Array.isArray(user.hideableInfo.capabilities)
                        ? user.hideableInfo.capabilities
                        : user.hideableInfo.capabilities.split(',').map(skill => skill.trim()),
                },
            };

            const response = await api.post(`/api/User/${user.id}`, updatedUser, {
                headers: {
                    Authorization: `Bearer ${token}`, // Include the token in the request headers
                },
            });

            console.log('Response from server after saving changes:', response.data); // Debug: Log server response
            setEditing(false);
        } catch (err) {
            setError('Failed to update user data');
        }
    };

    return (
        <div style={styles.container}>
            <h1 style={styles.title}>{user.name} {user.surname}'s Personal Information</h1>
            {!editing ? (
                <>
                    <p><strong>Email:</strong> {user.email}</p>
                    <p><strong>Position:</strong> {user.hideableInfo.currentPosition || 'No Position'}</p>
                    <p><strong>Location:</strong> {user.hideableInfo.location || 'No Location'}</p>
                    <p><strong>Phone:</strong> {user.hideableInfo.phoneNumber || 'No Phone Number'}</p>
                    <p><strong>Skills:</strong> {user.hideableInfo.capabilities?.length > 0 ? user.hideableInfo.capabilities.join(', ') : 'None'}</p>
                    <p><strong>Experience:</strong> {user.hideableInfo.experience?.length > 0 ? user.hideableInfo.experience.join(', ') : 'None'}</p>
                    <p><strong>Education:</strong> {user.hideableInfo.education?.length > 0 ? user.hideableInfo.education.join(', ') : 'None'}</p>
                    <p><strong>Public Information:</strong> {user.hideableInfo.currentPositionIsPublic ? "Yes" : "No"}</p>

                    {(isOwnProfile || isAdmin) && <button style={styles.button} onClick={() => setEditing(true)}>Edit Information</button>}
                </>
            ) : (
                <>
                    {(isOwnProfile || isAdmin) && (
                        <>
                            <div>
                                <label style={styles.label}>
                                    Email:
                                    <input type="email" name="email" value={user.email} onChange={handleInputChange} style={styles.input} />
                                </label>
                            </div>
                            <div>
                                <label style={styles.label}>
                                    Position:
                                    <input type="text" name="currentPosition" value={user.hideableInfo.currentPosition || ''} onChange={handleInputChange} style={styles.input} />
                                </label>
                            </div>
                            <div>
                                <label style={styles.label}>
                                    Location:
                                    <input type="text" name="location" value={user.hideableInfo.location || ''} onChange={handleInputChange} style={styles.input} />
                                </label>
                            </div>
                            <div>
                                <label style={styles.label}>
                                    Phone:
                                    <input type="text" name="phoneNumber" value={user.hideableInfo.phoneNumber || ''} onChange={handleInputChange} style={styles.input} />
                                </label>
                            </div>
                            <div>
                                <label style={styles.label}>
                                    Skills:
                                    <textarea
                                        name="capabilities"
                                        value={user.hideableInfo.capabilities ? user.hideableInfo.capabilities.join(', ') : ''}
                                        onChange={(e) => handleInputChange({ target: { name: 'capabilities', value: e.target.value.split(', ') } })}
                                        style={styles.textarea}
                                    />
                                </label>
                            </div>
                            <div>
                                <label style={styles.label}>
                                    Public Information:
                                    <input type="checkbox" name="currentPosition" checked={user.hideableInfo.currentPositionIsPublic} onChange={handleCheckboxChange} />
                                </label>
                            </div>
                            <button style={styles.button} onClick={saveChanges}>Save</button>
                            <button style={styles.button} onClick={() => setEditing(false)}>Cancel</button>
                        </>
                    )}
                </>
            )}
        </div>
    );
};

const styles = {
    container: {
        padding: '2rem',
        maxWidth: '600px',
        margin: '0 auto',
        backgroundColor: '#f9f9f9',
        borderRadius: '8px',
        boxShadow: '0 2px 10px rgba(0, 0, 0, 0.1)',
    },
    title: {
        fontSize: '1.8rem',
        marginBottom: '1.5rem',
        color: '#333',
        textAlign: 'center',
    },
    label: {
        display: 'block',
        margin: '0.5rem 0',
        color: '#555',
    },
    input: {
        width: '100%',
        padding: '0.5rem',
        borderRadius: '4px',
        border: '1px solid #ccc',
        boxSizing: 'border-box',
    },
    textarea: {
        width: '100%',
        padding: '0.5rem',
        borderRadius: '4px',
        border: '1px solid #ccc',
        height: '100px',
        boxSizing: 'border-box',
    },
    button: {
        margin: '1rem 0.5rem',
        padding: '0.5rem 1rem',
        border: 'none',
        borderRadius: '4px',
        backgroundColor: '#007BFF',
        color: 'white',
        cursor: 'pointer',
        transition: 'background-color 0.3s ease',
    },
    buttonHover: {
        backgroundColor: '#0056b3',
    },
    loading: {
        textAlign: 'center',
        fontSize: '1.5rem',
        margin: '2rem 0',
    },
    error: {
        color: 'red',
        textAlign: 'center',
        fontSize: '1.2rem',
        margin: '2rem 0',
    },
};

export default PersonalInfoPage;
