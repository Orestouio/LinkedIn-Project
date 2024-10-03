import React, { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import api from '../services/api';

const AdsPage = () => {
    const { user } = useAuth();
    const [userJobs, setUserJobs] = useState([]);
    const [otherJobs, setOtherJobs] = useState([]);
    const [newJob, setNewJob] = useState({ title: '', description: '', company: '', requirements: [], postFiles: [] });
    const [error, setError] = useState(null);
    const [loading, setLoading] = useState(true);
    const [successMessage, setSuccessMessage] = useState(null);

    // Helper function to get token
    const getAuthHeaders = () => {
        const token = localStorage.getItem('token');
        return { Authorization: `Bearer ${token}` };
    };

    // Fetch jobs on component mount and when user changes
    useEffect(() => {
        const fetchJobs = async () => {
            if (!user) {
                setLoading(false);
                return;
            }

            try {
                const [userJobsResponse, otherJobsResponse] = await Promise.all([
                    api.get(`/api/Job/by/${user.id}`, { headers: getAuthHeaders() }),
                    api.get('/api/Job', { headers: getAuthHeaders() })
                ]);

                setUserJobs(userJobsResponse.data);
                setOtherJobs(otherJobsResponse.data);
            } catch (err) {
                setError('Failed to fetch jobs.');
                console.error('Error fetching jobs:', err.response ? err.response.data : err.message);
            } finally {
                setLoading(false);
            }
        };

        fetchJobs();
    }, [user]);

    // Handle form field changes
    const handleNewJobChange = (e) => {
        const { name, value } = e.target;
        setNewJob((prevJob) => ({ ...prevJob, [name]: value }));
    };

    // Handle adding and editing requirements
    const handleAddRequirement = () => {
        setNewJob((prevJob) => ({
            ...prevJob,
            requirements: [...prevJob.requirements, ''] // Add empty string for new requirement
        }));
    };

    const handleRequirementChange = (e, index) => {
        const updatedRequirements = newJob.requirements.map((req, i) =>
            i === index ? e.target.value : req
        );
        setNewJob((prevJob) => ({ ...prevJob, requirements: updatedRequirements }));
    };

    // Handle job posting
    const handleNewJobSubmit = async (e) => {
        e.preventDefault();

        if (!user) {
            setError('No user found. Please log in.');
            return;
        }

        const jobData = {
            title: newJob.title,
            description: newJob.description,
            company: newJob.company,
            requirements: newJob.requirements,
            postFiles: newJob.postFiles
        };

        try {
            console.log(jobData);
            const response = await api.post(`/api/Job/by/${user.id}`, jobData, {
                headers: getAuthHeaders()
            });

            if (response.status === 200) {
                const userJobsResponse = await api.get(`/api/Job`, {
                    headers: getAuthHeaders()
                });
                setUserJobs(userJobsResponse.data);
                setSuccessMessage('Job posted successfully!');
                setNewJob({ title: '', description: '', company: '', requirements: [], postFiles: [] });
            } else {
                throw new Error('Failed to post new job.');
            }
        } catch (err) {
            setError(err.response ? err.response.data.message : 'Failed to post new job.');
            console.error('Error posting new job:', err.response ? err.response.data : err.message);
        }
    };

    // Handle job deletion
    const handleDeleteJob = async (jobId) => {
        if (!user) {
            setError('No user found. Please log in.');
            return;
        }

        try {
            await api.delete(`/api/Job/${jobId}`, { headers: getAuthHeaders() });
            setUserJobs((prevJobs) => prevJobs.filter(job => job.id !== jobId));
            setSuccessMessage('Job deleted successfully!');
        } catch (err) {
            setError(err.response ? err.response.data.message : 'Failed to delete job.');
            console.error('Error deleting job:', err.response ? err.response.data : err.message);
        }
    };

    // Handle setting interest in a job
    const handleSetInterest = async (jobId) => {
        if (!user) {
            setError('No user found. Please log in.');
            return;
        }

        try {
            await api.post(`/api/Job/${jobId}/interest/set/${user.id}`, {}, {
                headers: getAuthHeaders()
            });
            setSuccessMessage('Interest set successfully!');
        } catch (err) {
            setError(err.response ? err.response.data.message : 'Failed to set interest.');
            console.error('Error setting interest:', err.response ? err.response.data : err.message);
        }
    };

    // Handle unsetting interest in a job
    const handleUnsetInterest = async (jobId) => {
        if (!user) {
            setError('No user found. Please log in.');
            return;
        }

        try {
            await api.post(`/api/Job/${jobId}/interest/unset/${user.id}`, {}, {
                headers: getAuthHeaders()
            });
            setSuccessMessage('Interest removed successfully!');
        } catch (err) {
            setError(err.response ? err.response.data.message : 'Failed to unset interest.');
            console.error('Error unsetting interest:', err.response ? err.response.data : err.message);
        }
    };

    // Dismiss success and error messages automatically after 3 seconds
    useEffect(() => {
        if (successMessage || error) {
            const timer = setTimeout(() => {
                setSuccessMessage(null);
                setError(null);
            }, 3000); // Dismiss after 3 seconds
            return () => clearTimeout(timer);
        }
    }, [successMessage, error]);

    if (loading) return <p>Loading...</p>;
    if (error) return <p style={{ color: 'red' }}>{error}</p>;

    return (
        <div style={{ padding: '2rem', backgroundColor: '#f9f9f9' }}>
            <h1 style={{ textAlign: 'center', color: '#333' }}>Your Jobs</h1>

            {successMessage && <p style={{ color: 'green', textAlign: 'center' }}>{successMessage}</p>}

            <section style={{ marginBottom: '2rem' }}>
                <h2 style={{ borderBottom: '2px solid #007bff', paddingBottom: '0.5rem' }}>Published Jobs</h2>
                <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: '1rem' }}>
                    {userJobs.map((job) => (
                        <div key={job.id} style={{ border: '1px solid #ccc', borderRadius: '8px', padding: '1rem', backgroundColor: '#fff', boxShadow: '0 2px 4px rgba(0, 0, 0, 0.1)' }}>
                            <h3 style={{ color: '#007bff' }}>{job.title}</h3>
                            <p><strong>Company:</strong> {job.company}</p>
                            <p>{job.description}</p>
                            <div style={{ display: 'flex', justifyContent: 'space-between', marginTop: '1rem' }}>
                                <button onClick={() => handleDeleteJob(job.id)} style={buttonStyle}>Delete</button>
                                <button onClick={() => handleSetInterest(job.id)} style={buttonStyle}>Set Interest</button>
                                <button onClick={() => handleUnsetInterest(job.id)} style={buttonStyle}>Unset Interest</button>
                            </div>
                        </div>
                    ))}
                </div>
            </section>

            <section style={{ marginBottom: '2rem' }}>
                <h2 style={{ borderBottom: '2px solid #007bff', paddingBottom: '0.5rem' }}>Post a New Job</h2>
                <form onSubmit={handleNewJobSubmit}>
                    <div>
                        <label>
                            Title:
                            <input
                                type="text"
                                name="title"
                                value={newJob.title}
                                onChange={handleNewJobChange}
                                required
                                style={inputStyle}
                            />
                        </label>
                    </div>
                    <div>
                        <label>
                            Company:
                            <input
                                type="text"
                                name="company"
                                value={newJob.company}
                                onChange={handleNewJobChange}
                                required
                                style={inputStyle}
                            />
                        </label>
                    </div>
                    <div>
                        <label>
                            Description:
                            <textarea
                                name="description"
                                value={newJob.description}
                                onChange={handleNewJobChange}
                                required
                                style={{ ...inputStyle, height: '100px' }}
                            />
                        </label>
                    </div>

                    <div>
                        <label>Requirements:</label>
                        {newJob.requirements.map((req, index) => (
                            <div key={index}>
                                <input
                                    type="text"
                                    value={req}
                                    onChange={(e) => handleRequirementChange(e, index)}
                                    style={inputStyle}
                                />
                            </div>
                        ))}
                        <button type="button" onClick={handleAddRequirement} style={buttonStyle}>Add Requirement</button>
                    </div>

                    <button type="submit" style={{ ...buttonStyle, width: '100%', marginTop: '1rem' }}>Post Job</button>
                </form>
            </section>

            <section>
                <h2 style={{ borderBottom: '2px solid #007bff', paddingBottom: '0.5rem' }}>Other Professionals' Jobs</h2>
                <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: '1rem' }}>
                    {otherJobs.map((job) => (
                        <div key={job.id} style={{ border: '1px solid #ccc', borderRadius: '8px', padding: '1rem', backgroundColor: '#fff', boxShadow: '0 2px 4px rgba(0, 0, 0, 0.1)' }}>
                            <h3 style={{ color: '#007bff' }}>{job.title}</h3>
                            <p><strong>Company:</strong> {job.company}</p>
                            <p>{job.description}</p>
                            <div style={{ display: 'flex', justifyContent: 'space-between', marginTop: '1rem' }}>
                                <button onClick={() => handleSetInterest(job.id)} style={buttonStyle}>Set Interest</button>
                                <button onClick={() => handleUnsetInterest(job.id)} style={buttonStyle}>Unset Interest</button>
                            </div>
                        </div>
                    ))}
                </div>
            </section>
        </div>
    );
};

// Define styles for buttons and inputs
const buttonStyle = {
    backgroundColor: '#007bff',
    color: '#fff',
    border: 'none',
    borderRadius: '5px',
    padding: '0.5rem 1rem',
    cursor: 'pointer',
    transition: 'background-color 0.3s',
    fontSize: '0.9rem'
};

const inputStyle = {
    width: '100%',
    padding: '0.5rem',
    margin: '0.5rem 0',
    borderRadius: '5px',
    border: '1px solid #ccc',
    fontSize: '0.9rem'
};

export default AdsPage;
