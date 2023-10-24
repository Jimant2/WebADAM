// eslint-disable-next-line no-unused-vars
import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

function LoginPage() {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [licenseFile, setLicenseFile] = useState(null);
    const navigation = useNavigate();

    useEffect(() => {
        if (document.cookie.includes('CookieAuth')) {
            navigation('/main');
        }
    }, [navigation]);

    const handleLicenseUpload = async (e) => {
        e.preventDefault();

        if (!licenseFile) {
            alert("No license selected");
            return;
        }

        const formData = new FormData();
        formData.append('licenseFile', licenseFile);

        const requestUrl = 'https://localhost:7074/MainController/uploadLicense'
        try {
            const response = await fetch(requestUrl, {
                method: 'POST',
                body: formData,
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

        const requestUrl = 'https://localhost:7074/AuthController/login';
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
                body: JSON.stringify(requestData)
            });

            if (response.status === 200) {
                navigation('/main');
            } else {
                const errorData = await response.text();
                console.error('Error logging in:', errorData);
                alert("Incorrect login credentials!")
            }
        } catch (error) {
            console.error('Error logging in:', error);
            alert("Error during login!")
        }
    };


    return (
        <div>
            <div>
                <input type="file" accept=".xml" onChange={e => setLicenseFile(e.target.files[0])} />
                <button onClick={handleLicenseUpload}>Upload License</button>
            </div>
            <form onSubmit={handleSubmit}>
                <input type="text" value={username} onChange={e => setUsername(e.target.value)} placeholder="Username" />
                <input type="password" value={password} onChange={e => setPassword(e.target.value)} placeholder="Password" />
                <button type="submit">Login</button>
            </form>
        </div>
    );
}

export default LoginPage;
