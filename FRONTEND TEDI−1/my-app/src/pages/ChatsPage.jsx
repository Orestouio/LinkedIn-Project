import React, { useState, useEffect, useCallback, useRef } from 'react';
import { useParams } from 'react-router-dom';
import api from '../services/api';

const ChatsPage = () => {
    const { id: loggedInUserId } = useParams();
    const [conversations, setConversations] = useState([]);
    const [messages, setMessages] = useState([]);
    const [newMessage, setNewMessage] = useState('');
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [selectedRecipient, setSelectedRecipient] = useState(null);
    const [userInfo, setUserInfo] = useState(null);
    const token = localStorage.getItem('token');
    const chatContainerRef = useRef(null);
    const [lastMessageId, setLastMessageId] = useState(null);

    // Color mapping for users
    const userColors = {
        '1': 'green',   // User 1 color
        '2': 'blue',    // User 2 color
        '3': 'red',     // User 3 color
        // Add more users and colors as needed
    };

    // Scroll to the bottom of the chat
    const scrollToBottom = () => {
        if (chatContainerRef.current) {
            chatContainerRef.current.scrollTop = chatContainerRef.current.scrollHeight;
        }
    };

    // Fetch conversations
    const fetchConversations = useCallback(async () => {
        try {
            const response = await api.get(`/api/Message/chat/members/${loggedInUserId}`, {
                headers: { Authorization: `Bearer ${token}` },
            });
            setConversations(response.data);
            setLoading(false);
        } catch (err) {
            setError('Failed to fetch conversations.');
            setLoading(false);
        }
    }, [loggedInUserId, token]);

    // Fetch messages for selected recipient
    const fetchMessages = useCallback(async (recipientId) => {
        try {
            const response = await api.get(`/api/Message/chat/${loggedInUserId}/${recipientId}`, {
                headers: { Authorization: `Bearer ${token}` },
            });

            const newMessages = response.data;
            if (newMessages.length > 0 && newMessages[0].id !== lastMessageId) {
                setMessages(newMessages);
                setLastMessageId(newMessages[0].id);
                setTimeout(scrollToBottom, 100);
            }

            setLoading(false);
        } catch (err) {
            setError('Failed to fetch messages.');
            setLoading(false);
        }
    }, [loggedInUserId, token, lastMessageId]);

    // Polling for new messages
    useEffect(() => {
        let interval;
        if (selectedRecipient) {
            fetchMessages(selectedRecipient);
            interval = setInterval(() => {
                fetchMessages(selectedRecipient);
            }, 2000);
        }
        return () => clearInterval(interval);
    }, [selectedRecipient, fetchMessages]);

    // Fetch conversations on component mount
    useEffect(() => {
        fetchConversations();
    }, [fetchConversations]);

    // Fetch user info of the selected recipient
    const fetchUserInfo = useCallback(async (recipientId) => {
        try {
            const response = await api.get(`/api/User/${recipientId}`, {
                headers: { Authorization: `Bearer ${token}` },
            });
            setUserInfo(response.data?.value);
        } catch (err) {
            setError('Failed to fetch user info.');
        }
    }, [token]);

    // Update user info when recipient is selected
    useEffect(() => {
        if (selectedRecipient) {
            fetchUserInfo(selectedRecipient);
        }
    }, [selectedRecipient, fetchUserInfo]);

    // Handle sending a new message
    const handleSendMessage = async (e) => {
        e.preventDefault();
        if (!newMessage.trim() || !selectedRecipient) return;

        try {
            await api.post(`/api/Message/send/${loggedInUserId}/${selectedRecipient}?content=${encodeURIComponent(newMessage)}`, {}, {
                headers: { Authorization: `Bearer ${token}` },
            });
            setNewMessage('');
            fetchMessages(selectedRecipient);
        } catch (err) {
            setError('Failed to send message.');
        }
    };

    if (loading) return <div style={styles.loading}>Loading...</div>;
    if (error) return <div style={styles.error}>{error}</div>;

    return (
        <div style={styles.container}>
            <div style={styles.conversationList}>
                <h2>Your Conversations</h2>
                <ul>
                    {conversations.map((conversation, index) => (
                        <li key={index} onClick={() => setSelectedRecipient(conversation.id)}>
                            {conversation.fullName || `${conversation.name} ${conversation.surname}`}
                        </li>
                    ))}
                </ul>
            </div>

            <div style={styles.chatArea}>
                {selectedRecipient ? (
                    <>
                        <h2>Chat with {userInfo ? `${userInfo.name} ${userInfo.surname}` : `User ${selectedRecipient}`}</h2>
                        <div id="chat-container" ref={chatContainerRef} style={styles.chatContainer}>
                            {messages.length > 0 ? (
                                messages.slice().reverse().map((message, index) => {
                                    const userId = message.sentBy?.id; // Get the user ID from the message
                                    const userColor = userColors[userId] || 'black'; // Get the color, default to black
                                    const isSender = userId === loggedInUserId;
                                    const isUser2 = userId === '2'; // Check if the message is from user 2

                                    return (
                                        <div
                                            key={index}
                                            style={{
                                                marginBottom: '1rem',
                                                textAlign: isUser2 ? 'right' : (isSender ? 'right' : 'left'), // Align user 2's messages to the right
                                            }}
                                        >
                                            <strong style={{ color: userColor }}>
                                                {message.sentBy?.fullName || 'Unknown Sender'}:
                                            </strong>
                                            <p
                                                style={{
                                                    display: 'inline-block',
                                                    padding: '10px',
                                                    borderRadius: '10px',
                                                    backgroundColor: isSender ? '#dcf8c6' : '#ececec',
                                                    maxWidth: '80%',
                                                    wordWrap: 'break-word',
                                                }}
                                            >
                                                {message.content}
                                            </p>
                                        </div>
                                    );
                                })
                            ) : (
                                <p>No messages in this conversation.</p>
                            )}
                        </div>

                        <form onSubmit={handleSendMessage} style={styles.messageForm}>
                            <input
                                type="text"
                                placeholder="Type a message..."
                                value={newMessage}
                                onChange={(e) => setNewMessage(e.target.value)}
                                style={styles.input}
                            />
                            <button type="submit" style={styles.button}>Send</button>
                        </form>
                    </>
                ) : (
                    <p>Select a conversation to view messages</p>
                )}
            </div>
        </div>
    );
};

const styles = {
    container: {
        display: 'flex',
        padding: '1rem',
    },
    conversationList: {
        width: '30%',
        borderRight: '1px solid #ccc',
        paddingRight: '1rem',
    },
    chatArea: {
        width: '70%',
        paddingLeft: '1rem',
    },
    chatContainer: {
        height: '300px',
        overflowY: 'scroll',
        border: '1px solid #ccc',
        padding: '1rem',
        backgroundColor: '#f9f9f9',
    },
    messageForm: {
        marginTop: '1rem',
        display: 'flex',
        alignItems: 'center',
    },
    input: {
        width: '80%',
        marginRight: '1rem',
        padding: '10px',
        borderRadius: '5px',
        border: '1px solid #ccc',
    },
    button: {
        width: '20%',
        padding: '10px',
        backgroundColor: '#0073b1',
        color: 'white',
        border: 'none',
        borderRadius: '5px',
        cursor: 'pointer',
        transition: 'background-color 0.3s ease',
    },
    loading: {
        textAlign: 'center',
        fontSize: '1.5rem',
    },
    error: {
        color: 'red',
        textAlign: 'center',
    },
};

export default ChatsPage;
