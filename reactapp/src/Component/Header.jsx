import React from 'react';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import Button from '@mui/material/Button';
import { ProjectTypesModal } from './ProjectTypesModal';
import { getAllDevices } from '@/Controller/APIController';
import { useState } from 'react';
import PropTypes from 'prop-types';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import './Header.css';

function Header({ onDeviceSelect, onLogout }) {
    const [anchorEl, setAnchorEl] = React.useState(null);
    const [modalOpen, setModalOpen] = useState(false);
    const [deviceNames, setDeviceNames] = useState([]);
    const navigate = useNavigate();

    const handleClickOpenProjectType = async () => {
        try {
            const names = await getAllDevices();
            setDeviceNames(names);
            setModalOpen(true);
        } catch (error) {
            console.error('Failed to fetch device names:', error);
        }
        handleClose();
    };

    const handleSelectDevice = (deviceName) => {
        if (deviceName && typeof onDeviceSelect === 'function') {
            onDeviceSelect(deviceName);
        }
        setModalOpen(false);
    };
    const handleClick = (event) => {
        setAnchorEl(event.currentTarget);
    };

    const handleClose = () => {
        setAnchorEl(null);
    };
  
    const handleLogout = async () => {
        try {
            await axios.post('/AuthController/logout');
            navigate('/');
            console.log('Logout successful');
            if (typeof onLogout === 'function') {
                onLogout();
            }
        } catch (error) {
            console.error('Logout failed', error);
        }
    };
    const handleUpload = async () => {
        const fileInputChangePromise = new Promise(resolve => {
            const fileInput = document.createElement('input');
            fileInput.type = 'file';
            fileInput.addEventListener('change', () => resolve(fileInput.files[0]));
            fileInput.click();
        });

        const file = await fileInputChangePromise;

        if (!file) {
            console.error("No file selected");
            return;
        }

        const dataType = window.prompt('Please enter the data type:');
        if (!dataType || dataType.trim() === '') {
            alert('Data type is required.');
            return;
        }

        const formData = new FormData();
        formData.append('file', file);

        const requestUrl = `/MainController/uploadFile?dataType=${encodeURIComponent(dataType)}`;

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
    const handleExport = async () => {
        const dataType = window.prompt('Please enter the data type:');

        if (!dataType || dataType.trim() === '') {
            alert('Data type is required.');
            return;
        }

        const requestUrl = `/MainController/exportData/${encodeURIComponent(dataType)}`;

        try {
            const response = await fetch(requestUrl, {
                method: 'GET',
            });

            if (response.ok) {
                const blob = await response.blob();

                const link = document.createElement('a');
                link.href = window.URL.createObjectURL(blob);
                link.download = `${dataType}_dataSets.json`;
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);

                alert('File exported');
            } else {
                const errorMessage = await response.text();
                alert(`Error exporting file: ${errorMessage}`);
            }
        } catch (error) {
            console.error('Error exporting file:', error);
            alert(`Error exporting file: ${error.message}`);
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
                onClose={handleClose}>
                <MenuItem onClick={handleClickOpenProjectType}>Open Project Type</MenuItem>
                <MenuItem onClick={handleUpload}>Upload Data</MenuItem>
                <MenuItem onClick={ handleExport }>Export</MenuItem>
                <MenuItem onClick={handleLogout}>Logout</MenuItem>
            </Menu>
            <div>

            </div>
            <ProjectTypesModal
                open={modalOpen}
                onClose={() => setModalOpen(false)}
                deviceNames={deviceNames}
                onSelect={handleSelectDevice} />
        </header>
    );
}
Header.propTypes = {
    onDeviceSelect: PropTypes.func.isRequired,
    onLogout: PropTypes.func,
};

export default Header;
