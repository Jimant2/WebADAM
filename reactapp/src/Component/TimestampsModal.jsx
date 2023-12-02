// eslint-disable-next-line no-unused-vars
import React, { useEffect } from 'react';
import PropTypes from 'prop-types';

const TimestampModal = ({ timestamps, onSelectTimestamp }) => {
    useEffect(() => {
        if (!timestamps || timestamps.length === 0) {
            return;
        }

        const newWindow = window.open('', '_blank', 'width=400,height=400,left=200,top=200');
        renderModalContent(newWindow);

        // Close the new window when the component unmounts
        return () => {
            if (newWindow) {
                newWindow.close();
            }
        };
    }, [timestamps]);

    const renderModalContent = (newWindow) => {
        if (!newWindow) {
            return;
        }
        newWindow.document.title = 'Timestamp Modal';

        // Create elements
        const modalContainer = newWindow.document.createElement('div');
        const header = newWindow.document.createElement('h3');
        const ul = newWindow.document.createElement('ul');

        // Set content
        header.textContent = 'Select a Timestamp';

        timestamps.forEach((timestamp) => {
            const li = newWindow.document.createElement('li');
            li.textContent = timestamp;

            // Use 'click' event to handle the timestamp selection
            li.addEventListener('click', () => {
                if (onSelectTimestamp) {
                    onSelectTimestamp(timestamp, newWindow);
                }
            });

            ul.appendChild(li);
        });

        // Create close button
        const closeButton = newWindow.document.createElement('button');
        closeButton.textContent = 'Close';
        closeButton.addEventListener('click', () => newWindow.close());

        // Append elements to modalContainer
        modalContainer.appendChild(header);
        modalContainer.appendChild(ul);
        modalContainer.appendChild(closeButton);

        // Append modalContainer to the modal body
        newWindow.document.body.appendChild(modalContainer);

        const cssLink = newWindow.document.createElement("link");
        cssLink.href = "/Modal.css"; // Change the path accordingly
        cssLink.rel = "stylesheet";
        cssLink.type = "text/css";
        newWindow.document.head.appendChild(cssLink);
    };

    return null;
};

TimestampModal.propTypes = {
    timestamps: PropTypes.arrayOf(PropTypes.string),
    onSelectTimestamp: PropTypes.func.isRequired,
};

export default TimestampModal;
