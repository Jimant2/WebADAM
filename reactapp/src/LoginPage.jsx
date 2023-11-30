// eslint-disable-next-line no-unused-vars
import React, { useState, useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import './LoginPage.css';
import logo from './assets/DAC-logo.png';
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

        const formData = new FormData();
        formData.append('licenseFile', selectedFile);

        const requestUrl = '/MainController/uploadLicense'
        try {
            const response = await fetch(requestUrl, {
                method: 'POST',
                body: formData,
                credentials: 'include'
            });

            if (response.status === 200) {
                alert("License uploaded!");
                const data = await response.json();
                console.log(data);
            } else {
                alert("Error uploading license!");
                const errorData = await response.text();
                console.error('Error uploading license:', errorData);
            }
        } catch (error) {
            console.error('Error uploading license:', error);
        }
    };


    const handleSubmit = async (e) => {
        e.preventDefault();

        const requestUrl = '/AuthController/login';
        const requestData = {
            username: username,
            password: password
        };

        try {
            const response = await fetch(requestUrl, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                    
                },
                body: JSON.stringify(requestData),
                credentials: 'include'
            });

            if (response.status === 200) {
                navigation('/main');
            } else {
                const errorData = await response.text();
                console.error('Error logging in:', errorData);
                alert("Error during login!")

            }
        } catch (error) {
            console.error('Error logging in:', error);
            alert("Error during login!")
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
