import axios from 'axios';

export async function fetchDatasetsByDataType(dataType) {
    const response = await fetch(`/MainController/dataSetByDataType/${dataType}`);

    if (response.ok) {
        const dataSets = await response.json();

        console.log('Received dataSets:', dataSets);

        if (!Array.isArray(dataSets) || dataSets.length === 0) {
            console.log('Unexpected dataSets format:', dataSets);
            return { data: [], dataType: '' };
        }

        const allData = dataSets.flatMap(dataSet =>
            dataSet.data.map(dataPoint => ({
                timestamp: new Date(dataPoint.timestamp).getTime(),
                [dataSet.dataType]: dataPoint.value
            }))
        );

        allData.sort((a, b) => a.timestamp - b.timestamp);
        return {
            data: allData,
            dataType: dataSets[0].dataType
        };
    } else {
        const errorMessage = `Failed to fetch datasets: ${await response.text()}`;
        throw new Error(errorMessage);
    }
}
export async function uploadProjectType(file) {
    try {
        const apiUrl = '/MainController/addDeviceDefinitions';

        const formData = new FormData();
        formData.append('deviceFile', file);

        const response = await fetch(apiUrl, {
            method: 'POST',
            body: formData,
        });

        if (response.ok) {
            console.log('File uploaded successfully');
        } else {
            console.error('Failed to upload file');
        }
    } catch (error) {
        console.error('Error uploading file', error);
        throw new Error('Error uploading file');
    }
}
export async function uploadFile(file, dataType) {
    try {
        const formData = new FormData();
        formData.append('file', file);

        const apiUrl = `/MainController/uploadFile?dataType=${encodeURIComponent(dataType)}`;

        const response = await fetch(apiUrl, {
            method: 'POST',
            body: formData,
        });

        if (response.ok) {
            console.log('File uploaded successfully');
        } else {
            const errorData = await response.json();
            throw new Error(`Error uploading file: ${errorData.message || 'Unknown error'}`);
        }
    } catch (error) {
        console.error('Error uploading file:', error);
        throw new Error(`Error uploading file: ${error.message}`);
    }
}

export async function exportData(dataType) {
    try {
        const apiUrl = `/MainController/exportData/${encodeURIComponent(dataType)}`;

        const response = await fetch(apiUrl, {
            method: 'GET',
        });

        if (response.ok) {
            return await response.blob();
        } else {
            const errorMessage = await response.text();
            throw new Error(`Error exporting file: ${errorMessage}`);
        }
    } catch (error) {
        console.error('Error exporting file:', error);
        throw new Error(`Error exporting file: ${error.message}`);
    }
}

export async function fetchDatasetsByTimestamp(timestamp) {
    const response = await fetch(`/MainController/dataSetByTimestamp/${timestamp}`);

    if (response.ok) {
        const dataSets = await response.json();

        console.log('Received dataSets:', dataSets);

        if (!Array.isArray(dataSets) || dataSets.length === 0) {
            console.log('Unexpected dataSets format:', dataSets);
            return { data: [], dataType: '' };
        }

        const allData = dataSets.flatMap(dataSet =>
            dataSet.data.map(dataPoint => ({
                timestamp: new Date(dataPoint.timestamp).getTime(),
                [dataSet.dataType]: dataPoint.value
            }))
        );

        allData.sort((a, b) => a.timestamp - b.timestamp);
        return {
            data: allData,
            dataType: dataSets[0].dataType
        };
    } else {
        const errorMessage = `Failed to fetch datasets: ${await response.text()}`;
        throw new Error(errorMessage);
    }
}

export async function fetchTimestampsByDataType(dataType) {
    try {
        const response = await fetch(`/MainController/timestampByDataType/${dataType}`);
        if (response.ok) {
            console.log(`Fetching timestamps by data type`, response);
            const data = await response.json() || [];
            return data;
        }
    } catch (error) {
        console.error("Failed to grab the timestamps: ", error);
        throw error;
    }
}

export async function fetchGroupsAndChannels(deviceName) {
    try {
        const requestUrl = `/MainController/getGroupsAndChannels/?deviceName=${deviceName}`;
        const response = await fetch(requestUrl);
        console.log('Fetched groups and channels:', response);
        const data = await response.json() || [];
        return data; 
    } catch (error) {
        console.error("Failed fetching groups and channels:", error);
        throw error;  
    }
}

export async function getAllDevices() {
    const response = await fetch('/MainController/getAllDevices');
    if (response.headers.get("content-type")?.includes("application/json")) {
        return await response.json();
    } else {
        const errorMessage = `Received non-JSON response: ${await response.text()}`;
        throw new Error(errorMessage);
    }
}



export async function getDeviceByName(deviceName) {
        const requestUrl = `/MainController/getDeviceByName/?deviceName=${deviceName}`;
        const response = await fetch(requestUrl);
        if (response.headers.get("content-type")?.includes("application/json")) {
            return await response.json();
        } else {
            const errorMessage = `Received non-JSON response: ${await response.text()}`;
            throw new Error(errorMessage);
        }
}
export async function logout() {
    try {
        await axios.post('/AuthController/logout');
        console.log('Logout successful');
    } catch (error) {
        console.error('Logout failed', error);
        throw new Error('Logout failed');
    }
}
