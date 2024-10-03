import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import api from '../services/api'; // Import the API instance

const NotificationsPage = () => {
    const { id } = useParams();  // Get userId from URL params
    const [requests, setRequests] = useState([]);  // Store connection requests
    const [notifications, setNotifications] = useState([]);  // Store post notifications
    const [loading, setLoading] = useState(true);  // Track loading state
    const [error, setError] = useState(null);  // Track errors

    useEffect(() => {
        const fetchNotifications = async () => {
            const token = localStorage.getItem('token');  // Fetch token from localStorage
            if (!token || !id) {
                console.error('No token or user ID found. Please log in again.');
                setError('No token or user ID found. Please log in again.');
                setLoading(false);
                return;
            }

            try {
                // Fetch connection requests
                console.log(`Fetching connection requests for user with ID: ${id}`);
                const requestsResponse = await api.get(`/api/Connection/received/${id}`, {
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                });

                const filteredRequests = requestsResponse.data.filter((request) => request.name && request.surname);
                if (filteredRequests.length > 0) {
                    setRequests(filteredRequests);
                } else {
                    setRequests([]);  // No valid requests found
                }

                // Fetch notifications about posts
                console.log(`Fetching post notifications for user with ID: ${id}`);
                const notificationsResponse = await api.get(`/api/Notification/my/${id}`, {
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                });

                setNotifications(notificationsResponse.data);  // Set the post notifications
                setLoading(false);
            } catch (err) {
                console.error('Error fetching data:', err.response ? err.response.data : err.message);
                setError('Failed to fetch data.');
                setLoading(false);
            }
        };

        fetchNotifications();
    }, [id]);

    const handleAcceptRequest = async (requestId) => {
        const token = localStorage.getItem('token');
        if (!token) {
            setError('No token found. Please log in again.');
            return;
        }

        try {
            console.log(`Accepting connection request with ID: ${requestId}`);
            await api.post(`/api/Connection/accept/${requestId}`, {}, {
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            });

            // Refresh the list after accepting the request
            setRequests(requests.filter((req) => req.id !== requestId));
        } catch (err) {
            console.error('Error accepting connection request:', err.response ? err.response.data : err.message);
            setError('Failed to accept the connection request.');
        }
    };

    const handleDeclineRequest = async (requestId) => {
        const token = localStorage.getItem('token');
        if (!token) {
            setError('No token found. Please log in again.');
            return;
        }

        try {
            console.log(`Declining connection request with ID: ${requestId}`);
            await api.post(`/api/Connection/decline/${requestId}`, {}, {
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            });

            // Refresh the list after declining the request
            setRequests(requests.filter((req) => req.id !== requestId));
        } catch (err) {
            console.error('Error declining connection request:', err.response ? err.response.data : err.message);
            setError('Failed to decline the connection request.');
        }
    };

    const handleMarkAsRead = async (notificationId) => {
        const token = localStorage.getItem('token');
        if (!token) {
            setError('No token found. Please log in again.');
            return;
        }

        try {
            console.log(`Marking notification with ID: ${notificationId} as read`);
            await api.post(`/api/Notification/read/${notificationId}`, {}, {
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            });

            // Refresh the list by removing the marked notification or updating its state
            setNotifications(notifications.map((notification) =>
                notification.id === notificationId ? { ...notification, read: true } : notification
            ));
        } catch (err) {
            console.error('Error marking notification as read:', err.response ? err.response.data : err.message);
            setError('Failed to mark the notification as read.');
        }
    };

    if (loading) return <div style={styles.loading}>Loading...</div>;
    if (error) return <div style={styles.error}>{error}</div>;

    return (
        <div style={styles.container}>
            <h1 style={styles.title}>Notifications</h1>

            {/* Display Connection Requests */}
            {requests.length > 0 ? (
                <div>
                    <h2 style={styles.subTitle}>Connection Requests</h2>
                    <ul style={styles.list}>
                        {requests.map((request) => (
                            <li key={request.id} style={styles.listItem}>
                                <p>
                                    <strong>{request.name} {request.surname}</strong> wants to connect with you.
                                    <br />
                                    <button style={styles.button} onClick={() => handleAcceptRequest(request.id)}>Accept</button>
                                    <button style={styles.button} onClick={() => handleDeclineRequest(request.id)}>Decline</button>
                                </p>
                            </li>
                        ))}
                    </ul>
                </div>
            ) : (
                <p>No connection requests.</p>  // Display this message when no more valid requests are left
            )}

            {/* Display Post Notifications */}
            {notifications.length > 0 ? (
                <div>
                    <h2 style={styles.subTitle}>Post Notifications</h2>
                    <ul style={styles.list}>
                        {notifications.map((notification) => (
                            <li key={notification.id} style={{ ...styles.listItem, opacity: notification.read ? 0.5 : 1 }}>
                                <p>{notification.content}</p>
                                <p><em>{new Date(notification.timestamp).toLocaleString()}</em></p>
                                {!notification.read && (
                                    <button style={styles.button} onClick={() => handleMarkAsRead(notification.id)}>Mark as Read</button>
                                )}
                            </li>
                        ))}
                    </ul>
                </div>
            ) : (
                <p>No new notifications.</p>  // Display this message when there are no notifications
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
        fontSize: '2rem',
        marginBottom: '1rem',
        textAlign: 'center',
        color: '#333',
    },
    subTitle: {
        fontSize: '1.5rem',
        margin: '1rem 0',
        color: '#555',
    },
    list: {
        listStyleType: 'none',
        padding: 0,
    },
    listItem: {
        padding: '1rem',
        borderBottom: '1px solid #ccc',
    },
    button: {
        margin: '0 0.5rem',
        padding: '0.5rem 1rem',
        border: 'none',
        borderRadius: '4px',
        backgroundColor: '#007BFF',
        color: 'white',
        cursor: 'pointer',
        transition: 'background-color 0.3s ease',
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

export default NotificationsPage;
