import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import api from '../services/api';

const HomePage = () => {
    const { user } = useAuth();
    const [newPost, setNewPost] = useState('');
    const [posts, setPosts] = useState([]);
    const [likesModalVisible, setLikesModalVisible] = useState(false);
    const [currentPostLikes, setCurrentPostLikes] = useState([]);

    useEffect(() => {
        const fetchPosts = async () => {
            const token = localStorage.getItem('token');
            try {
                const response = await api.get(`/api/Post/timeline/suggested/${user.id}`, {
                    headers: { Authorization: `Bearer ${token}` },
                });
                const formattedPosts = response.data.map(post => ({
                    ...post,
                    comments: post.comments || [],
                    postedBy: post.postedBy || { name: 'Unknown' },
                    likes: post.likes || 0,
                    likedBy: post.likedBy || [], // Assuming the API returns an array of users who liked the post
                }));
                setPosts(formattedPosts);
            } catch (error) {
                console.error('Error fetching posts:', error);
            }
        };

        if (user) {
            fetchPosts();
        }
    }, [user]);

    const handlePostSubmit = async (event) => {
        event.preventDefault();
        const token = localStorage.getItem('token');
        const postData = {
            content: newPost,
            postedBy: { id: user.id, name: user.name, imagePath: user.imagePath },
            postedAt: new Date().toISOString(),
        };

        try {
            const response = await api.post(`/api/Post/create/${user.id}`, postData, {
                headers: { Authorization: `Bearer ${token}` },
            });
            setPosts([{ ...response.data, likes: 0, likedBy: [], comments: [] }, ...posts]);
            setNewPost('');
        } catch (error) {
            console.error('Error creating post:', error);
        }
    };

    const handleLikePost = async (postId) => {
        const token = localStorage.getItem('token');
        const userId = user?.id;

        if (!userId) return;

        const currentPost = posts.find(post => post.id === postId);
        const liked = currentPost.likedBy.includes(userId);

        const newLikesCount = liked ? currentPost.likes - 1 : currentPost.likes + 1;
        const newLikedBy = liked 
            ? currentPost.likedBy.filter(id => id !== userId) 
            : [...currentPost.likedBy, userId];

        setPosts(posts.map(post =>
            post.id === postId
                ? { ...post, likes: newLikesCount, likedBy: newLikedBy }
                : post
        ));

        try {
            if (liked) {
                await api.post(`/api/Post/${postId}/interest/unset/${userId}`, {}, {
                    headers: { Authorization: `Bearer ${token}` },
                });
            } else {
                await api.post(`/api/Post/${postId}/interest/set/${userId}`, {}, {
                    headers: { Authorization: `Bearer ${token}` },
                });
            }
        } catch (error) {
            console.error('Error liking post:', error);
            setPosts(posts.map(post =>
                post.id === postId
                    ? { ...post, likes: currentPost.likes, likedBy: currentPost.likedBy }
                    : post
            ));
        }
    };

    const handleShowLikes = (postId) => {
        const currentPost = posts.find(post => post.id === postId);
        setCurrentPostLikes(currentPost.likedBy.map(userId => ({ id: userId, name: user.name }))); // Adjust this as needed
        setLikesModalVisible(true);
    };

    const handleAddComment = async (postId, commentText, isReply = false, parentCommentIndex = null) => {
        if (!commentText.trim()) return;
        const userId = user?.id;
        const token = localStorage.getItem('token');

        if (!userId) return;

        setPosts((prevPosts) => prevPosts.map((post) => {
            if (post.id === postId) {
                if (isReply) {
                    return {
                        ...post,
                        comments: post.comments.map((comm, idx) => {
                            if (idx === parentCommentIndex) {
                                return {
                                    ...comm,
                                    replies: [...(comm.replies || []), { name: user.name, comment: commentText }],
                                };
                            }
                            return comm;
                        }),
                    };
                } else {
                    return {
                        ...post,
                        comments: [...post.comments, { user: { name: user.name }, comment: commentText, replies: [] }],
                    };
                }
            }
            return post;
        }));

        try {
            await api.post(`/api/Post/reply/${postId}/${userId}`, { content: commentText }, {
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            });
        } catch (error) {
            console.error('Error adding comment:', error);
        }
    };

    return (
        <div style={styles.container}>
            {user ? (
                <>
                    <div style={styles.sidebar}>
                        <Link to={`/personal-info/${user.id}`} style={styles.link}>Personal Information</Link>
                        <Link to={`/network/${user.id}`} style={styles.link}>Network</Link>
                    </div>
                    <div style={styles.mainContent}>
                        <h1 style={styles.title}>Home Page</h1>
                        <form onSubmit={handlePostSubmit} style={styles.postForm}>
                            <textarea
                                value={newPost}
                                onChange={(e) => setNewPost(e.target.value)}
                                placeholder="What's on your mind?"
                                required
                                style={styles.textarea}
                            ></textarea>
                            <button type="submit" style={styles.postButton}>Post</button>
                        </form>

                        <div style={styles.timeline}>
                            {posts.length > 0 ? (
                                posts.map((post) => (
                                    <div key={post.id} style={styles.post}>
                                        <p style={styles.postContent}>{post.content}</p>
                                        <small style={styles.postDetails}>
                                            Posted by {post.postedBy?.name || 'Unknown'} on {new Date(post.postedAt).toLocaleString()}
                                        </small>

                                        <div style={styles.likeSection}>
                                            <button onClick={() => handleLikePost(post.id)} style={styles.likeButton}>
                                                {post.likedBy.includes(user.id) ? '👎 Unliked' : '👍 Like'}
                                            </button> 
                                            <span style={styles.likesCount}>{post.likes} Likes</span>
                                            <button onClick={() => handleShowLikes(post.id)} style={styles.showLikesButton}>View Likes</button>
                                        </div>

                                        <div style={styles.comments}>
                                            <h4 style={styles.commentTitle}>Comments</h4>
                                            {Array.isArray(post.comments) && post.comments.length > 0 ? (
                                                post.comments.map((comment, commentIndex) => (
                                                    <div key={commentIndex} style={styles.comment}>
                                                        <p style={styles.commentText}>
                                                            <strong>{comment.user.name || 'Unknown'}:</strong> 
                                                            {comment.comment}
                                                        </p>
                                                        <textarea
                                                            placeholder="Reply..."
                                                            onKeyDown={(e) => {
                                                                if (e.key === 'Enter' && !e.shiftKey) {
                                                                    e.preventDefault();
                                                                    handleAddComment(post.id, e.target.value, true, commentIndex);
                                                                    e.target.value = '';
                                                                }
                                                            }}
                                                            style={styles.replyTextarea}
                                                        ></textarea>
                                                    </div>
                                                ))
                                            ) : (
                                                <p>No comments yet.</p>
                                            )}

                                            <textarea
                                                placeholder="Add a comment..."
                                                onKeyDown={(e) => {
                                                    if (e.key === 'Enter' && !e.shiftKey) {
                                                        e.preventDefault();
                                                        handleAddComment(post.id, e.target.value);
                                                        e.target.value = '';
                                                    }
                                                }}
                                                style={styles.commentTextarea}
                                            ></textarea>
                                        </div>
                                    </div>
                                ))
                            ) : (
                                <p>No posts available.</p>
                            )}
                        </div>

                        {likesModalVisible && (
                            <div style={styles.modalOverlay}>
                                <div style={styles.modal}>
                                    <h2 style={styles.modalTitle}>Likes</h2>
                                    <ul style={styles.likesList}>
                                        {currentPostLikes.map((like, index) => (
                                            <li key={index} style={styles.likeItem}>{like.name}</li>
                                        ))}
                                    </ul>
                                    <button onClick={() => setLikesModalVisible(false)} style={styles.closeModalButton}>Close</button>
                                </div>
                            </div>
                        )}
                    </div>
                </>
            ) : (
                <p>Please log in to see your personalized content.</p>
            )}
        </div>
    );
};

const styles = {
    container: {
        display: 'flex',
        padding: '1rem',
    },
    sidebar: {
        width: '25%',
        borderRight: '1px solid #ccc',
        padding: '1rem',
        backgroundColor: '#f8f8f8', // Light background for better contrast
        borderRadius: '5px', // Rounded corners
        boxShadow: '2px 2px 5px rgba(0, 0, 0, 0.1)', // Subtle shadow for depth
    },
    link: {
        display: 'block', // Each link takes full width
        padding: '10px 15px', // Padding around links
        color: '#0073e6', // Link color (similar to LinkedIn)
        textDecoration: 'none', // Remove underline
        borderRadius: '3px', // Rounded corners for links
        transition: 'background-color 0.3s', // Transition for hover effect
    },
    linkHover: {
        backgroundColor: '#e6f7ff', // Light blue on hover
    },
    mainContent: {
        width: '75%',
        padding: '1rem',
    },
    title: {
        fontSize: '2rem',
    },
    postForm: {
        display: 'flex',
        flexDirection: 'column',
    },
    textarea: {
        height: '100px',
        marginBottom: '10px',
    },
    postButton: {
        padding: '10px',
        backgroundColor: 'blue',
        color: 'white',
        border: 'none',
        cursor: 'pointer',
    },
    timeline: {
        marginTop: '20px',
    },
    post: {
        marginBottom: '20px',
        border: '1px solid #ccc',
        borderRadius: '5px',
        padding: '10px',
    },
    postContent: {
        fontSize: '1.1rem',
    },
    postDetails: {
        fontSize: '0.9rem',
        color: '#555',
    },
    likeSection: {
        display: 'flex',
        alignItems: 'center',
    },
    likeButton: {
        marginRight: '10px',
        padding: '5px',
    },
    likesCount: {
        marginRight: '10px',
    },
    showLikesButton: {
        padding: '5px',
    },
    comments: {
        marginTop: '10px',
    },
    commentTitle: {
        fontSize: '1.2rem',
    },
    comment: {
        margin: '10px 0',
    },
    commentText: {
        marginBottom: '5px',
    },
    replyTextarea: {
        width: '100%',
        height: '50px',
    },
    commentTextarea: {
        width: '100%',
        height: '50px',
        marginTop: '10px',
    },
    modalOverlay: {
        position: 'fixed',
        top: 0,
        left: 0,
        right: 0,
        bottom: 0,
        backgroundColor: 'rgba(0, 0, 0, 0.5)',
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
    },
    modal: {
        backgroundColor: 'white',
        padding: '20px',
        borderRadius: '5px',
        maxWidth: '400px',
        width: '90%',
    },
    modalTitle: {
        fontSize: '1.5rem',
        marginBottom: '10px',
    },
    likesList: {
        listStyleType: 'none',
        padding: 0,
        margin: 0,
    },
    likeItem: {
        margin: '5px 0',
    },
    closeModalButton: {
        marginTop: '10px',
        padding: '5px',
    },
};


export default HomePage;
