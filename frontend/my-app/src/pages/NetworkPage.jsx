import React, { useState, useEffect } from 'react';
import { useHistory, useParams } from 'react-router-dom';
import api from '../services/api'; // Import the API instance

const NetworkPage = () => {
    const { id: loggedInUserId } = useParams();  // Get logged-in userId from URL params
    const [connections, setConnections] = useState([]);  // Store the connected professionals
    const [searchResults, setSearchResults] = useState([]);  // Store search results
    const [searchQuery, setSearchQuery] = useState('');  // Track search query
    const [error, setError] = useState(null);  // Track errors
    const [searchError, setSearchError] = useState(null);  // Track search-specific errors
    const history = useHistory();  // Used for navigation
    const token = localStorage.getItem('token');  // Fetch token from localStorage

    useEffect(() => {
        const fetchConnections = async () => {
            if (!token || !loggedInUserId) {
                setError('No token or user ID found. Please log in again.');
                return;
            }

            try {
                const response = await api.get(`/api/Connection/network/${loggedInUserId}`, {
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                });
                setConnections(response.data);
            } catch (err) {
                setError('Failed to fetch connections.');
            }
        };

        fetchConnections();
    }, [token, loggedInUserId]);

    // Handle search input change
    const handleSearchChange = (e) => {
        setSearchQuery(e.target.value);
    };

    // Handle search submit with basic validation
    const handleSearchSubmit = async (e) => {
        e.preventDefault();

        if (!isNaN(searchQuery) || searchQuery.trim().length < 3) {
            setSearchError('Please enter a valid search term (at least 3 characters long and not a number).');
            return;
        }

        if (!token) {
            setError('No token found. Please log in again.');
            return;
        }

        try {
            const response = await api.get(`/api/User/search/${searchQuery}`, {
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            });
            setSearchResults(response.data);
            setSearchError(null);
        } catch (err) {
            setSearchError('Failed to fetch search results.');
        }
    };

    // Navigate to personal info page with the searched user ID and the logged-in user ID
    const handleNavigateToProfile = (searchedUserId) => {
        history.push(`/personal-info/${loggedInUserId}/${searchedUserId}`);
    };

    // Navigate to chat with a professional
    const handleStartChat = (professionalId) => {
        history.push(`/chat/${loggedInUserId}/${professionalId}`);  // Pass both logged-in user ID and the professional's ID
    };


    return (
        <div style={{ padding: '1rem' }}>
            <h1>Your Professional Network</h1>

            {/* Search Box */}
            <form onSubmit={handleSearchSubmit}>
                <input
                    type="text"
                    placeholder="Search for professionals..."
                    value={searchQuery}
                    onChange={handleSearchChange}
                />
                <button type="submit">Search</button>
            </form>

            {searchError && <p style={{ color: 'red' }}>{searchError}</p>}
            {error && <p style={{ color: 'red' }}>{error}</p>}

            {/* Grid of Connected Professionals */}
            <h2>Connected Professionals</h2>
            {connections.length > 0 ? (
                <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: '1rem' }}>
                    {connections.map((professional) => (
                        <div key={professional.id} style={{ border: '1px solid #ccc', padding: '1rem' }}>
                            <img
                                src={professional.imagePath || 'placeholder.jpg'}
                                alt={professional.name}
                                style={{ width: '100px', height: '100px', objectFit: 'cover' }}
                            />
                            <h3>{professional.name} {professional.surname}</h3>
                            <p>{professional.currentPosition}</p>
                            <p>{professional.location}</p>
                            <button onClick={() => handleNavigateToProfile(professional.id)}>View Profile</button>
                            <button onClick={() => handleStartChat(professional.id)}>Start Chat</button>
                        </div>
                    ))}
                </div>
            ) : (
                <p>No connections found.</p>
            )}

            {/* Search Results */}
            {searchResults.length > 0 && (
                <>
                    <h2>Search Results</h2>
                    <ul>
                        {searchResults.map((professional) => (
                            <li key={professional.id}>
                                <strong>{professional.name} {professional.surname}</strong> - {professional.currentPosition} at {professional.location}
                                <br />
                                <button onClick={() => handleNavigateToProfile(professional.id)}>
                                    {professional.isConnected ? 'View Full Profile' : 'View Public Info'}
                                </button>
                            </li>
                        ))}
                    </ul>
                </>
            )}
        </div>
    );
};

export default NetworkPage;
