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
