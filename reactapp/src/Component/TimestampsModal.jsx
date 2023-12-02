/* eslint-disable no-unused-vars */
import React, { useEffect } from 'react';
import PropTypes from 'prop-types';

const TimestampModal = ({ timestamps, onSelectTimestamp }) => {
    useEffect(() => {
        const newWindow = window.open('', '_blank', 'width=400,height=400,left=200,top=200');
        renderModalContent(newWindow);

        // Close the new window when the component unmounts
        return () => {
            if (newWindow) {
                newWindow.close();
            }
        };
    }, [timestamps, onSelectTimestamp]);

    const renderModalContent = (newWindow) => {
        if (!newWindow) {
            return;
        }

        newWindow.document.title = 'Timestamp Modal';

        // Render the modal content in the new window
        newWindow.document.body.innerHTML = (
            <div>
                <h3>Select a Timestamp:</h3>
                <ul>
                    {timestamps.map((timestamp, index) => (
                        <li key={index} onClick={() => onSelectTimestamp(timestamp, newWindow)}>
                            {timestamp}
                        </li>
                    ))}
                </ul>
                <button onClick={() => newWindow.close()}>Close</button>
            </div>
        );
    };

    // No need to render anything in the component
    return null;
};

TimestampModal.propTypes = {
    timestamps: PropTypes.arrayOf(PropTypes.string),
    onSelectTimestamp: PropTypes.func.isRequired,
};

export default TimestampModal;