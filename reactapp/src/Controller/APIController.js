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
        return data;  // return data instead of calling this.setState
    } catch (error) {
        console.error("Failed fetching groups and channels:", error);
        throw error;  // re-throw to be caught in the calling component
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
