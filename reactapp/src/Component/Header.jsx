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
        if (!file) return;

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
        formData.set('file', file);
        formData.set('deviceName', deviceName);
        formData.set('dataType', dataType);
        try {
            const response = await fetch('https://localhost:7074/MainController/upload', {
                method: 'POST',
                body: formData,
            });
            console.log(formData.file);

            if (response.ok) {
                console.log('File uploaded successfully');
            } else {
                console.error('Error uploading file:', await response.json());
            }
        } catch (error) {
            console.error('Error uploading file:', error);
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
