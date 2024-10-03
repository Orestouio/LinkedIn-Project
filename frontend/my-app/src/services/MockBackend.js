// src/services/MockBackend.js
const users = [
    {
        id: 1,
        name: "John Doe",
        photo: "https://via.placeholder.com/150",
        position: "Software Engineer",
        company: "Tech Inc",
        email: "john@example.com",
        phone: "123-456-7890",
        password: "password1",
        isConnected: true,
        publicInfo: true,
        posts: ["Post 1", "Post 2"],
        ads: ["Ad 1", "Ad 2"],
        experience: "5 years in software development",
        interestNotes: ["Interested in AI", "Interested in Web Development"],
        comments: ["Comment 1 on Post A", "Comment 2 on Post B"],
        network: ["Jane Smith", "Robert Johnson"],
        role: "user",  // Regular user role
    },
    {
        id: 2,
        name: "Jane Smith",
        photo: "https://via.placeholder.com/150",
        position: "Product Manager",
        company: "Business Corp",
        email: "jane@example.com",
        phone: "987-654-3210",
        password: "password2",
        isConnected: true,
        publicInfo: true,
        posts: ["Post A", "Post B"],
        ads: ["Ad X", "Ad Y"],
        experience: "3 years in product management",
        interestNotes: ["Interested in marketing", "Interested in user experience"],
        comments: ["Comment A on Post 1", "Comment B on Post 2"],
        network: ["John Doe", "Sam Wilson"],
        role: "user",  // Regular user role
    },
    {
        id: 3,
        name: "Admin User",
        photo: "https://via.placeholder.com/150",
        position: "Administrator",
        company: "Tech Inc",
        email: "admin@example.com",
        phone: "000-000-0000",
        password: "adminpass",
        isConnected: true,
        publicInfo: true,
        posts: [],
        ads: [],
        experience: "Administrative role",
        interestNotes: [],
        comments: [],
        network: [],
        role: "admin",  // Admin role
    },
];

export const getUsers = () => {
    // Filter out the admin user from the list of users
    return users.filter(user => user.role !== 'admin');
};

export const getUserById = (id) => users.find((user) => user.id === id);

export const searchUsers = (query) => {
    if (!query) return [];
    return users.filter(
        (user) =>
            user.name.toLowerCase().includes(query.toLowerCase()) ||
            user.position.toLowerCase().includes(query.toLowerCase()) ||
            user.company.toLowerCase().includes(query.toLowerCase())
    );
};

// New function to authenticate user
export const authenticateUser = (email, password) => {
    return users.find(user => user.email === email && user.password === password);
};
