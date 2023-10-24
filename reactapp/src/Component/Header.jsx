import React from 'react';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import Button from '@mui/material/Button';
import { ProjectTypesModal } from './ProjectTypesModal';
import { getAllDevices } from '@/Controller/APIController';
import { useState } from 'react';
import PropTypes from 'prop-types';


function Header({ onDeviceSelect }) {
    const [anchorEl, setAnchorEl] = React.useState(null);
    const [modalOpen, setModalOpen] = useState(false);
    const [deviceNames, setDeviceNames] = useState([]);
    //const fileInputRef = React.useRef(null);

    const handleClickOpenProjectType = async () => {
        try {
            const names = await getAllDevices();
            setDeviceNames(names);
            setModalOpen(true);
        } catch (error) {
            console.error('Failed to fetch device names:', error);
        }
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

    //const handleOpenFile = () => {
    //    fileInputRef.current.click();
    //    handleClose();
    //};

    //const handleExport = () => {

    //};

    {/*Deprecated method*/ }

    //const handleFileChange = async (e) => {
    //    const file = e.target.files[0];
    //    if (!file) {
    //        console.error("No file selected");
    //        return;
    //    }

    //    const dataType = window.prompt('Please enter the data type:');
    //    if (!dataType || dataType.trim() === '') {
    //        alert('Data type is required.');
    //        return;
    //    }

    //    const formData = new FormData();
    //    formData.append('file', file);

    //    const requestUrl = `https://localhost:7074/MainController/uploadFile?dataType=${encodeURIComponent(dataType)}`;

    //    try {
    //        const response = await fetch(requestUrl, {
    //            method: 'POST',
    //            body: formData,
    //        });

    //        if (response.ok) {
    //            console.log('File uploaded successfully');
    //            alert("File uploaded!")
    //        } else {
    //            console.error('Error response:', response);
    //            const errorData = await response.json();
    //            console.error('Error data:', errorData);
    //            alert(`Error uploading file: ${errorData.message || 'Unknown error'}`);
    //        }
    //    } catch (error) {
    //        console.error('Error uploading file:', error);
    //        alert(`Error uploading file: ${error.message}`);
    //    }
    //};


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
                <MenuItem onClick={handleClose}>Save</MenuItem>
                <MenuItem onClick={() => { }}>Export</MenuItem>
            </Menu>

            <ProjectTypesModal
                open={modalOpen}
                onClose={() => setModalOpen(false)}
                deviceNames={deviceNames}
                onSelect={handleSelectDevice} />
        </header>
    );
}
Header.propTypes = {
    onDeviceSelect: PropTypes.func.isRequired
};

export default Header;
