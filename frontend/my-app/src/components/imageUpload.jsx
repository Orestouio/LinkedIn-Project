// ./components/ImageUpload.jsx
import React, { useState } from 'react';
import api from '../services/api'; 

const ImageUpload = ({ onUploadSuccess }) => {
    const [file, setFile] = useState(null); 
    const [uploadMessage, setUploadMessage] = useState(''); 

   
    const handleFileChange = (event) => {
        setFile(event.target.files[0]); 
    };

    // Handles the upload of the image
    const handleUpload = async (event) => {
    
        event.preventDefault();

        if (!file) {
            alert('Please select a file to upload.');
            return;
        }

        const formData = new FormData();
        formData.append('image', file); 
        console.log(formData)
        try {
            // Fetch the token for authorization
            const token = localStorage.getItem('token');
            
            // Send the image using a POST request to your backend
            const response = await api.post('/api/Image/upload', formData, {
                headers: {
                    Authorization: `Bearer ${token}`, 
                },
            });
            console.log(response);

            // Check if the upload was successful
            if (response.status === 200) {
                setUploadMessage('Image uploaded successfully.');
                onUploadSuccess(response.data); 
            } else {
                setUploadMessage('Image upload failed.');
            }
        } catch (error) {
            console.error('Image upload error:', error);
            setUploadMessage('Failed to upload image.'); 
        }
    };

    return (
        <div className="image-upload">
            <h2>Upload Image</h2>
            <form onSubmit={handleUpload}>
                <input type="file" onChange={handleFileChange} required /> {/* File input */}
                <button type="submit" className="btn">Upload</button> {/* Submit button */}
            </form>
            {uploadMessage && <p>{uploadMessage}</p>} {/* Display upload status message */}
        </div>
        
    );
};

export default ImageUpload;
