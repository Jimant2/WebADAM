// eslint-disable-next-line no-unused-vars
import React, { useEffect, useState, useRef } from 'react';
import PropTypes from 'prop-types';
import ReactDOM from 'react-dom';

export function ProjectTypesModal({ open, onClose, deviceNames, onSelect }) {
    const [selectedDevice, setSelectedDevice] = useState(null);
    const externalWindowRef = useRef(null);
    const containerElRef = useRef(document.createElement('div'));

    useEffect(() => {
        if (open) {
            externalWindowRef.current = window.open('', '', 'width=600,height=400,left=200,top=200');
            const cssLink = document.createElement("link");
            cssLink.href = "/Modal.css";
            cssLink.rel = "stylesheet";
            cssLink.type = "text/css";
            externalWindowRef.current.document.head.appendChild(cssLink);
            externalWindowRef.current.document.body.appendChild(containerElRef.current);
            externalWindowRef.current.addEventListener('beforeunload', () => {
                onClose();
            });
        }
        return () => {
            if (externalWindowRef.current) {
                externalWindowRef.current.close();
            }
        };
    }, [open]);

    const handleDeviceClick = (device) => {
        if (device) {
            setSelectedDevice(device);
        }
    };

    const content = (
        <div>
            <h2 className="project-types-title">Project Types</h2>
            <ul className="devices-list">
                {deviceNames.map(name => (
                    <li
                        key={name}
                        onClick={() => handleDeviceClick(name)}
                        style={{ cursor: 'pointer', backgroundColor: selectedDevice === name ? '#e0e0e0' : 'transparent' }}
                    >
                        {name}
                    </li>
                ))}
            </ul>
            <button className="select-button" onClick={() => {
                if (selectedDevice) {
                    onSelect(selectedDevice);
                    console.log("Selected device:" + selectedDevice);
                    externalWindowRef.current.close();
                } else {
                    console.error("No device selected");
                }}}>OK</button>
        </div>
    );

    return open ? ReactDOM.createPortal(content, containerElRef.current) : null;
}

ProjectTypesModal.propTypes = {
    open: PropTypes.bool.isRequired,
    onClose: PropTypes.func.isRequired,
    deviceNames: PropTypes.arrayOf(PropTypes.string).isRequired,
    onSelect: PropTypes.func.isRequired
};
