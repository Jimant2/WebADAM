// eslint-disable-next-line no-unused-vars
import React, { useState } from 'react';
import axios from 'axios';

function LoginComponent() {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const response = await axios.post('/api/auth/login', { username, password });
            if (response.status === 200) {
                // Handle successful login, e.g., redirect to dashboard
            }
        } catch (error) {
            // Handle login error
        }
    };

    return (
        <form onSubmit={handleSubmit}>
            <input type="text" value={username} onChange={e => setUsername(e.target.value)} placeholder="Username" />
            <input type="password" value={password} onChange={e => setPassword(e.target.value)} placeholder="Password" />
            <button type="submit">Login</button>
        </form>
    );
}

export default LoginComponent;
