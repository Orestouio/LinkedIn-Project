import React from 'react';
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';
import Navbar from './components/Navbar';
import WelcomePage from './pages/WelcomePage';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import HomePage from './pages/HomePage';
import AdminDashboard from './pages/AdminDashboard';
import UserList from './pages/UserList';
import UserDetails from './pages/UserDetails';
import InfoPage from './pages/PersonalInfo';
import NetworkPage from './pages/NetworkPage';
import Settings from './pages/SettingsPage';
import NotificationsPage from './pages/NotificationsPage';
import Ads from './pages/AdsPage';
import ChatsPage from './pages/ChatsPage'; // Ensure ChatsPage is correctly imported
import { AuthProvider } from './context/AuthContext';

const App = () => {
    return (
        <AuthProvider>
            <Router>
                <Navbar />
                <Switch>
                    <Route exact path="/" component={WelcomePage} />
                    <Route path="/login" component={LoginPage} />
                    <Route path="/register" component={RegisterPage} />
                    <Route path="/home/:id" component={HomePage} />
                    <Route exact path="/admin/:id" component={AdminDashboard} />
                    <Route exact path="/admin/:id/users" component={UserList} />
                    <Route exact path="/admin/:id/users/:userId" component={UserDetails} />
                    <Route path="/personal-info/:userId/:searchedUserId?" component={InfoPage} />
                    <Route path="/network/:id" component={NetworkPage} />
                    <Route path="/ads/:id" component={Ads} />
                    <Route path="/notifications/:id" component={NotificationsPage} />
                    <Route exact path="/chat/:id" component={ChatsPage} /> {/* Chat List */}
                    <Route path="/chat/:id/:recipientId" component={ChatsPage} /> {/* Specific Chat */}
                    <Route path="/settings/:id" component={Settings} />
                </Switch>
            </Router>
        </AuthProvider>
    );
};

export default App;
