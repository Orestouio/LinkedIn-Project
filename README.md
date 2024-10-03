
# Professional Networking Application

**Developers:**
- Marios-Triantafyllos Karnavas (sdi1900232)
- Konstantinos Merkouris (sdi1800319)
- Orestis Theodorou (sdi2000058)

**Spring Semester 2023-2024**

## Table of Contents:
1. [Introduction](#1-introduction)
2. [Installation Instructions](#2-installation-instructions)
   - [Backend Installation](#21-backend-installation)
     - PostgreSQL Installation
     - .NET Framework Installation
     - Application Installation
     - Creating Dummy Data and Running the Backend
   - [Frontend Installation](#22-frontend-installation)
     - React and Dependencies Installation
     - Environment Variables Setup (Frontend)
     - Starting the Frontend
3. [Backend Development](#3-backend-development)
   - Models Development
   - API Context Development
   - Services Development
   - LINQ
   - Controllers Development
   - Authentication
   - Authorization
4. [Frontend Development](#4-frontend-development)
   - Overview of Project Structure
   - Running the Application
   - Navbar Component
   - Network Page
   - Admin Dashboard
   - Chats Page
   - Notifications Page
5. [Conclusion](#5-conclusion)
   - Difficulties and Solutions
   - Conflicts with the Original Specifications

---

## 1. Introduction

The goal of this project is to develop a LinkedIn-style social networking application. It includes a backend offering a RESTful API and a frontend for easier user interaction.

**Technologies Used:**
- **PostgreSQL**: Robust and flexible database.
- **ASP.NET**: Backend development with authenticated and authorized requests.
- **React**: Frontend development.

---

## 2. Installation Instructions

### 2.1 Backend Installation

#### 2.1.1 PostgreSQL Installation
Follow the instructions in this [video](#) to install PostgreSQL. Make sure to remember your username and password for later. Use PGAdmin to create a `linkedout` database on `localhost`.

#### 2.1.2 .NET Framework Installation
Download the .NET 8.0 SDK from [here](https://dotnet.microsoft.com/).

#### 2.1.3 Application Installation
Ensure the following:
- HTTP and HTTPS ports 5048 and 8043 are free.
- The HTTPS certificate is trusted by your machine.
- The connection string matches your PostgreSQL setup.

Once ready, run:
```bash
dotnet ef database update --context ApiContext
```

#### 2.1.4 Creating Dummy Data and Running the Backend
Run the application with initial data by uncommenting lines 168-170 in the backend code and execute it.

### 2.2 Frontend Installation

#### 2.2.1 React and Dependencies Installation
Prerequisites:
- **Node.js** (v12 or newer)
- **npm**

Navigate to the frontend directory:
```bash
cd FRONTEND_TEDI-1/my-app
npm install
```

#### 2.2.2 Environment Variables Setup (Frontend)
Create `api.js` in `my-app/src/services` and set the base URL for the API:
```javascript
const baseURL = "http://localhost:5048";
```

#### 2.2.3 Starting the Frontend
Start the frontend:
```bash
cd FRONTEND_TEDI-1/my-app
npm start
```

---

## 3. Backend Development

The backend is built using **ASP.NET Core** and connected to a **PostgreSQL** database. Key parts of the development include:

- **Models**: Represent the database structure.
- **API Context**: The interface between code and database.
- **Services**: Handle CRUD operations and more complex logic.
- **Controllers**: Manage endpoints for user interaction (Login, Register, etc.).
- **Authentication**: Managed via JSON Web Tokens (JWT).
- **Authorization**: Based on custom policies.

---

## 4. Frontend Development

### 4.1 Overview of Project Structure
- **src/components**: Contains all React components.
- **src/pages**: Represents different pages (Home, Chat, etc.).
- **src/context**: Global context for authentication handling.
- **src/services**: Includes `api.js` for HTTP requests to the backend.

### 4.2 Running the Application
The frontend is designed to run on `http://localhost:5048`.

- **Navbar**: Dynamically changes based on login status and role.
- **Network Page**: Allows users to manage professional connections.
- **Admin Dashboard**: Admins can view and export user data in JSON or XML formats.
- **Chats Page**: Real-time messaging between users.
- **Notifications Page**: Manage connection requests and other notifications.

---

## 5. Conclusion

### 5.1 Difficulties and Solutions
- **Challenge 1**: Lack of native mathematical matrix support in C#. We built the matrix operations from scratch.
- **Challenge 2**: Managing complex LINQ queries. We used lazy loading to simplify.
- **Challenge 3**: Customizing policies for authorization in ASP.NET. We created unique policies for each request type.

### 5.2 Conflicts with the Original Specifications
- The frontend makes HTTP requests instead of HTTPS.
- The backend does not support audio media storage, only video and images.
- The frontend does not yet support uploading media to the backend.
