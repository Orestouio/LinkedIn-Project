import axios from 'axios';

const api = axios.create({
    baseURL: 'http://localhost:5048', // Local development URL
    headers: {
        'Content-Type': 'application/json',
    },
});

export default api;
