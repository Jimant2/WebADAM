import React from 'react';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import Button from '@mui/material/Button';

function Header() {
    const [anchorEl, setAnchorEl] = React.useState(null);
    const fileInputRef = React.useRef(null);

    const handleClick = (event) => {
        setAnchorEl(event.currentTarget);
    };

    const handleClose = () => {
        setAnchorEl(null);
    };

    const handleOpenFile = () => {
        fileInputRef.current.click();
        handleClose();
    };

    const handleExport = () => {

    };

    const handleFileChange = async (e) => {
        const file = e.target.files[0];
        if (!file) {
            console.error("No file selected");
            return;
        }

        const deviceName = window.prompt('Please enter the device name:');
        if (!deviceName || deviceName.trim() === '') {
            alert('Device name is required.');
            return;
        }

        const dataType = window.prompt('Please enter the data type:');
        if (!dataType || dataType.trim() === '') {
            alert('Data type is required.');
            return;
        }

        const formData = new FormData();
        formData.append('file', file);

        const requestUrl = `https://localhost:7074/MainController/uploadFile?deviceName=${encodeURIComponent(deviceName)}&dataType=${encodeURIComponent(dataType)}`;

        try {
            const response = await fetch(requestUrl, {
                method: 'POST',
                body: formData,
            });

            if (response.ok) {
                console.log('File uploaded successfully');
                alert("File uploaded!")
            } else {
                console.error('Error response:', response);
                const errorData = await response.json();
                console.error('Error data:', errorData);
                alert(`Error uploading file: ${errorData.message || 'Unknown error'}`);
            }
        } catch (error) {
            console.error('Error uploading file:', error);
            alert(`Error uploading file: ${error.message}`);
        }
    };


    return (
        <header>
            <Button className="file-Button" onClick={handleClick}>
                File
            </Button>
            <Menu
                anchorEl={anchorEl}
                keepMounted
                open={Boolean(anchorEl)}
                onClose={handleClose}
            >
                <MenuItem onClick={handleOpenFile}>Open</MenuItem>
                <MenuItem onClick={handleClose}>Save</MenuItem>
                <MenuItem onClick={handleExport}>Export</MenuItem>
            </Menu>

            <input
                type="file"
                style={{ display: 'none' }}
                ref={fileInputRef}
                onChange={handleFileChange}
            />
        </header>
    );
}

export default Header;
