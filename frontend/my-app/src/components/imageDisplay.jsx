import React, { useState, useEffect } from 'react';
import api from '../services/api';

const ImageDisplay = ({ imageName }) => {
    const [imageUrl, setImageUrl] = useState('');

    useEffect(() => {
        const fetchImage = async () => {
            try {
                const response = await api.get(`/api/Image/${imageName}`);
                setImageUrl(response.data.imageUrl); // Adjust based on your response structure
            } catch (error) {
                console.error('Error fetching image:', error);
            }
        };

        if (imageName) {
            fetchImage();
        }
    }, [imageName]);

    return (
        <div className="image-display">
            {imageUrl ? (
                <img src={imageUrl} alt={imageName} />
            ) : (
                <p>No image available.</p>
            )}
        </div>
    );
};

export default ImageDisplay;
