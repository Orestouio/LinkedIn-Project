import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import api from '../services/api';
import { toXML } from 'jstoxml';

const UserDetails = () => {
    const { userId } = useParams();  // Get the userId from the URL parameters
    const [user, setUser] = useState(null);
    const [posts, setPosts] = useState([]); // User posts
    const [interestedPosts, setInterestedPosts] = useState([]); // Posts the user is interested in
    const [jobsPosted, setJobsPosted] = useState([]); // Jobs the user has posted
    const [interestedJobs, setInterestedJobs] = useState([]); // Jobs the user is interested in
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        const fetchUserData = async () => {
            const token = localStorage.getItem('token');  // Retrieve the token from localStorage

            if (!token) {
                setError('No token found. Please log in again.');
                setLoading(false);
                return;
            }

            try {
                // Fetch user details
                const userResponse = await api.get(`/api/User/${userId}`, {
                    headers: {
                        Authorization: `Bearer ${token}`,  // Add token to the request headers
                    },
                });
                setUser(userResponse.data.value);

                // Fetch posts made by the user
                const postsResponse = await api.get(`/api/Post/from/${userId}?includeReplies=true`, {
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                });
                setPosts(postsResponse.data);

                // Fetch posts the user is interested in
                const interestedPostsResponse = await api.get(`/api/Post/interested/${userId}`, {
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                });
                setInterestedPosts(interestedPostsResponse.data);

                // Fetch jobs posted by the user
                const jobsPostedResponse = await api.get(`/api/Job/by/${userId}`, {
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                });
                setJobsPosted(jobsPostedResponse.data);

                // Fetch jobs the user is interested in
                const interestedJobsResponse = await api.get(`/api/Job/interested/${userId}`, {
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                });
                setInterestedJobs(interestedJobsResponse.data);

                setLoading(false);
            } catch (err) {
                console.error('Error fetching data:', err);
                setError('Failed to fetch data');
                setLoading(false);
            }
        };

        fetchUserData();
    }, [userId]);

    const handleExport = (format) => {
        if (!user) return;

        const exportData = {
            user,
            posts,
            interestedPosts,
            jobsPosted,
            interestedJobs,
        };

        if (format === 'json') {
            const jsonData = JSON.stringify(exportData, null, 2);
            const blob = new Blob([jsonData], { type: 'application/json' });
            const url = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.setAttribute('download', `user_${userId}_data.json`);
            document.body.appendChild(link);
            link.click();
        } else if (format === 'xml') {
            const config = {
                indent: '    '
            };
            const xmlContent = toXML(exportData, config);

            const blob = new Blob([xmlContent], { type: 'application/xml' });
            const url = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.setAttribute('download', `user_${userId}_data.xml`);
            document.body.appendChild(link);
            link.click();
        }
    };



    if (loading) return <div>Loading...</div>;
    if (error) return <div>{error}</div>;

    return (
        <div>
            <h1>User Details</h1>
            {user ? (
                <div>
                    <p><strong>Email:</strong> {user.email || 'Not available'}</p>
                    <p><strong>Full Name:</strong> {user.fullName || 'Not available'}</p>
                    <p><strong>User Role:</strong> {user.userRole || 'Not available'}</p>

                    <h2>Hideable Info</h2>
                    <p><strong>Phone Number:</strong> {user.hideableInfo?.phoneNumber || 'Not available'}</p>
                    <p><strong>Location:</strong> {user.hideableInfo?.location || 'Not available'}</p>
                    <p><strong>Experience:</strong> {user.hideableInfo?.experience?.join(', ') || 'Not available'}</p>
                    <p><strong>Capabilities:</strong> {user.hideableInfo?.capabilities?.join(', ') || 'Not available'}</p>
                    <p><strong>Education:</strong> {user.hideableInfo?.education?.join(', ') || 'Not available'}</p>


                    <h2>User Posts</h2>
                    {posts.length > 0 ? (
                        <ul>
                            {posts.map(post => (
                                <li key={post.id}>
                                    <p><strong>Content:</strong> {post.content}</p>
                                    <p><strong>Posted At:</strong> {new Date(post.postedAt).toLocaleString()}</p>
                                </li>
                            ))}
                        </ul>
                    ) : <p>No posts found for this user.</p>}

                    <h2>Posts Interested In</h2>
                    {interestedPosts.length > 0 ? (
                        <ul>
                            {interestedPosts.map(post => (
                                <li key={post.id}>
                                    <p><strong>Content:</strong> {post.content}</p>
                                    <p><strong>Posted At:</strong> {new Date(post.postedAt).toLocaleString()}</p>
                                </li>
                            ))}
                        </ul>
                    ) : <p>No interested posts found for this user.</p>}

                    <h2>Jobs Posted</h2>
                    {jobsPosted.length > 0 ? (
                        <ul>
                            {jobsPosted.map(job => (
                                <li key={job.id}>
                                    <p><strong>Title:</strong> {job.jobTitle}</p>
                                    <p><strong>Description:</strong> {job.description}</p>
                                </li>
                            ))}
                        </ul>
                    ) : <p>No jobs posted by this user.</p>}

                    <h2>Jobs Interested In</h2>
                    {interestedJobs.length > 0 ? (
                        <ul>
                            {interestedJobs.map(job => (
                                <li key={job.id}>
                                    <p><strong>Title:</strong> {job.title}</p>
                                    <p><strong>Description:</strong> {job.description}</p>
                                </li>
                            ))}
                        </ul>
                    ) : <p>No jobs interested by this user.</p>}

                    <button onClick={() => handleExport('json')}>Export as JSON</button>
                    <button onClick={() => handleExport('xml')}>Export as XML</button>
                </div>
            ) : (
                <p>User not found</p>
            )}
        </div>
    );
};

export default UserDetails;
