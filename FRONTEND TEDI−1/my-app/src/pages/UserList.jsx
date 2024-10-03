import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { getUsers } from '../services/MockBackend'; // Import the getUsers function from your mock backend

const UserList = () => {
    const users = getUsers(); // Fetch the users from the mock backend
    const [selectedUsers, setSelectedUsers] = useState([]); // State to store selected users

    // Handle selecting or deselecting users
    const toggleUserSelection = (user) => {
        if (selectedUsers.includes(user)) {
            setSelectedUsers(selectedUsers.filter(u => u.id !== user.id));
        } else {
            setSelectedUsers([...selectedUsers, user]);
        }
    };

    // Convert selected users to JSON and trigger download
    const exportAsJSON = () => {
        if (selectedUsers.length === 0) {
            alert('No users selected for export.');
            return;
        }
        const json = JSON.stringify(selectedUsers, null, 2);
        const blob = new Blob([json], { type: 'application/json' });
        const url = URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = 'users.json';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    };

    // Convert selected users to XML and trigger download
    const exportAsXML = () => {
        if (selectedUsers.length === 0) {
            alert('No users selected for export.');
            return;
        }
        const xml = generateXML(selectedUsers);
        const blob = new Blob([xml], { type: 'application/xml' });
        const url = URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = 'users.xml';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    };

    // Helper function to generate XML from user data
    const generateXML = (users) => {
        const xmlItems = users.map(user => `
            <user>
                <id>${user.id}</id>
                <name>${user.name}</name>
                <email>${user.email}</email>
                <phone>${user.phone}</phone>
                <position>${user.position}</position>
                <company>${user.company}</company>
                <experience>${user.experience}</experience>
                <interestNotes>${user.interestNotes.join(', ')}</interestNotes>
                <comments>${user.comments.join(', ')}</comments>
                <network>${user.network.join(', ')}</network>
            </user>
        `).join('');
        return `<users>${xmlItems}</users>`;
    };

    return (
        <div style={styles.container}>
            <h1 style={styles.title}>User Management</h1>
            <div style={styles.tableContainer}>
                <table style={styles.table}>
                    <thead>
                        <tr>
                            <th>Select</th>
                            <th>Name</th>
                            <th>Email</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {users.map((user) => (
                            <tr key={user.id}>
                                <td>
                                    <input
                                        type="checkbox"
                                        checked={selectedUsers.includes(user)}
                                        onChange={() => toggleUserSelection(user)}
                                    />
                                </td>
                                <td>{user.name}</td>
                                <td>{user.email}</td>
                                <td style={styles.actionCell}>
                                    {/* Update the link to redirect to personal-info */}
                                    <Link to={`/personal-info/${user.id}`} style={styles.link}>View Details</Link>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
            <div style={styles.exportContainer}>
                <button style={styles.exportButton} onClick={exportAsXML}>Export as XML</button>
                <button style={styles.exportButton} onClick={exportAsJSON}>Export as JSON</button>
            </div>
        </div>
    );
};

const styles = {
    container: {
        padding: '20px',
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
    },
    title: {
        marginBottom: '20px',
    },
    tableContainer: {
        width: '100%',
        display: 'flex',
        justifyContent: 'center',
    },
    table: {
        width: '80%',
        borderCollapse: 'collapse',
    },
    actionCell: {
        display: 'flex',
        justifyContent: 'center',
    },
    link: {
        marginRight: '10px',
        padding: '5px 10px',
        backgroundColor: '#007bff',
        color: 'white',
        textDecoration: 'none',
        borderRadius: '5px',
    },
    exportContainer: {
        marginTop: '20px',
        display: 'flex',
        justifyContent: 'center',
        gap: '10px',
    },
    exportButton: {
        padding: '10px 20px',
        backgroundColor: '#007bff',
        color: 'white',
        border: 'none',
        borderRadius: '5px',
        cursor: 'pointer',
    },
};

export default UserList;
