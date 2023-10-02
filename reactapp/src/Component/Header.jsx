import React from 'react';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import Button from '@mui/material/Button';

function Header() {
    const [anchorEl, setAnchorEl] = React.useState(null);

    const handleClick = (event) => {
        setAnchorEl(event.currentTarget);
    };

    const handleClose = () => {
        setAnchorEl(null);
    };

    return (
        <header>
            <Button onClick={handleClick}>
                File
            </Button>
            <Menu
                anchorEl={anchorEl}
                keepMounted
                open={Boolean(anchorEl)}
                onClose={handleClose}
            >
                <MenuItem onClick={handleClose}>Open</MenuItem>
                <MenuItem onClick={handleClose}>Save</MenuItem>
            </Menu>

            {/* Similar Blocks for Edit and other menu items can be added here */}

        </header>
    );
}

export default Header;
