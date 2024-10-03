import React, { useState, useEffect } from 'react';
import { Link, useParams } from 'react-router-dom';
import api from '../services/api';
import { toXML } from 'jstoxml';

const AdminDashboard = () => {
    const { id: adminId } = useParams();  // Get the admin ID from the URL parameters
    const [users, setUsers] = useState([]);
    const [selectedUsers, setSelectedUsers] = useState([]);  // Track selected users
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        const fetchUsers = async () => {
            const token = localStorage.getItem('token');  // Retrieve token from localStorage

            if (!token) {
                setError('No token found. Please log in again.');
                setLoading(false);
                return;
            }

            try {
                const response = await api.get('/api/User', {
                    headers: {
                        Authorization: `Bearer ${token}`,  // Add the token to the request
                    },
                });
                setUsers(response.data);  // Set users data
                setLoading(false);
            } catch (err) {
                setError('Failed to fetch users');
                setLoading(false);
            }
        };

        fetchUsers();
    }, []);

    const handleUserSelection = (userId) => {
        setSelectedUsers((prevSelected) =>
            prevSelected.includes(userId)
                ? prevSelected.filter((id) => id !== userId)
                : [...prevSelected, userId]
        );
    };

    const handleExportMultiple = async (format) => {
        if (!selectedUsers.length) {
            alert('You have not selected any users.');
            return;  // Return if no users are selected
        }

        const token = localStorage.getItem('token');
        if (!token) return;

        try {
            const exportData = await Promise.all(
                selectedUsers.map(async (userId) => {
                    const userResponse = await api.get(`/api/User/${userId}`, {
                        headers: { Authorization: `Bearer ${token}` },
                    });

                    const postsResponse = await api.get(`/api/Post/from/${userId}?includeReplies=true`, {
                        headers: { Authorization: `Bearer ${token}` },
                    });

                    const interestedPostsResponse = await api.get(`/api/Post/interested/${userId}`, {
                        headers: { Authorization: `Bearer ${token}` },
                    });

                    const jobsPostedResponse = await api.get(`/api/Job/by/${userId}`, {
                        headers: { Authorization: `Bearer ${token}` },
                    });

                    const interestedJobsResponse = await api.get(`/api/Job/interested/${userId}`, {
                        headers: { Authorization: `Bearer ${token}` },
                    });

                    return {
                        user: userResponse.data.value,
                        posts: postsResponse.data,
                        interestedPosts: interestedPostsResponse.data,
                        jobsPosted: jobsPostedResponse.data,
                        interestedJobs: interestedJobsResponse.data,
                    };
                })
            );

            if (format === 'json') {
                const jsonData = JSON.stringify(exportData, null, 2);
                const blob = new Blob([jsonData], { type: 'application/json' });
                const url = window.URL.createObjectURL(blob);
                const link = document.createElement('a');
                link.href = url;
                link.setAttribute('download', `Linked-Users.json`);
                document.body.appendChild(link);
                link.click();
            } else if (format === 'xml') {
                const config = {
                    indent: '    '
                };
                const xmlContent = toXML({ users: exportData }, config);
                const blob = new Blob([xmlContent], { type: 'application/xml' });
                const url = window.URL.createObjectURL(blob);
                const link = document.createElement('a');
                link.href = url;
                link.setAttribute('download', `Linked-Users.xml`);
                document.body.appendChild(link);
                link.click();
            }
        } catch (err) {
            console.error('Failed to export data for selected users:', err);
        }
    };


    if (loading) return <div>Loading...</div>;
    if (error) return <div>{error}</div>;

    return (
        <div>
            <h1>Manage Users</h1>
            <table style={styles.table}>
                <thead>
                    <tr>
                        <th style={styles.th}>Select ID</th>
                        <th style={styles.th}>Name</th>
                        <th style={styles.th}>Email</th>
                        <th style={styles.th}>Current Position</th>
                        <th style={styles.th}>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {users.map((user) => (
                        <tr key={user.id}>
                            <td style={styles.tdCenter}>
                                <input
                                    type="checkbox"
                                    checked={selectedUsers.includes(user.id)}
                                    onChange={() => handleUserSelection(user.id)}
                                />
                            </td>
                            <td style={styles.td}>{`${user.name} ${user.surname}`}</td>
                            <td style={styles.tdCenter}>{user.email}</td>
                            <td style={styles.td}>{user.hideableInfo.currentPosition}</td>
                            <td style={styles.tdCenter}>
                                <Link to={`/admin/${adminId}/users/${user.id}`} style={styles.link}>
                                    View Details
                                </Link>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>

            {/* Export selected users' data as JSON */}
            <button onClick={() => handleExportMultiple('json')} style={styles.button}>
                Export Selected as JSON
            </button>

            {/* Export selected users' data as XML */}
            <button onClick={() => handleExportMultiple('xml')} style={styles.button}>
                Export Selected as XML
            </button>
        </div>
    );
};

// Basic styling for better layout and alignment
const styles = {
    table: {
        width: '90%',
        borderCollapse: 'collapse',
        marginTop: '20px',
        margin: '0 auto', // Center the table
    },
    th: {
        border: '1px solid #ddd',
        padding: '12px',
        textAlign: 'left',
        backgroundColor: '#f2f2f2',
        fontWeight: 'bold',
    },
    td: {
        border: '1px solid #ddd',
        padding: '12px',
        textAlign: 'left',
    },
    tdCenter: {
        border: '1px solid #ddd',
        padding: '12px',
        textAlign: 'center',
    },
    button: {
        marginTop: '20px',
        padding: '10px 15px',
        backgroundColor: '#007bff',
        color: 'white',
        border: 'none',
        borderRadius: '5px',
        cursor: 'pointer',
        display: 'block',
        marginLeft: 'auto',
        marginRight: 'auto', // Center the button
    },
    link: {
        color: '#007bff',
        textDecoration: 'underline',
    },
};

export default AdminDashboard;
