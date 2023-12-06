// eslint-disable-next-line no-unused-vars
import React, { useState, useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import './LoginPage.css';
import logo from './assets/DAC-logo.png';
import { handleLicenseUpload, handleLogin } from './Controller/APIController';
function LoginPage() {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const navigation = useNavigate();
    const fileInput = useRef(null);

    useEffect(() => {
        if (document.cookie.includes('CookieAuth')) {
            navigation('/main');
        }
    }, [navigation]);

    const handleFileSelection = async (e) => {
        const selectedFile = e.target.files[0];
        if (!selectedFile) {
            alert("No license selected");
            return;
        }

        try {
            const data = await handleLicenseUpload(selectedFile);
            alert("License uploaded!");
            console.log(data);
        } catch (error) {
            alert("Error uploading license!");
            console.error('Error uploading license:', error.message);
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            const response = await handleLogin(username, password);

            if (response.ok) {
                navigation('/main');
            } else {
                const errorData = await response.text();
                console.error('Error logging in:', errorData);
                alert("Error during login!");
            }
        } catch (error) {
            console.error('Error logging in:', error);
            alert("Error during login!");
        }
    };


    return (
        <div className="login-container">
            <div className="imageContainer">
                <img src={logo} alt="DAC Logo"></img>
            </div>
            <div className="license-upload">
                <input type="file" accept=".xml" style={{ display: 'none' }} ref={fileInput} onChange={handleFileSelection} />
                <button onClick={() => fileInput.current.click()}>Upload License</button>
            </div>
            <form className="login-form" onSubmit={handleSubmit}>
                <input type="text" value={username} onChange={e => setUsername(e.target.value)} placeholder="Username" />
                <input type="password" value={password} onChange={e => setPassword(e.target.value)} placeholder="Password" />
                <button type="submit">Login</button>
            </form>
        </div>
    );

}

export default LoginPage;
