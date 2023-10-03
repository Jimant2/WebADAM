export async function fetchDatasetsForDevice(deviceName) {
    const response = await fetch(`https://localhost:7074/MainController/dataSetByName/${deviceName}`);
    if (response.ok) {
        const dataSets = await response.json();
        if (dataSets.length === 0) {
            return { data: [], dataType: '' };
        }

        // Map through all datasets and combine the data
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



export async function getDevice() {
    const response = await fetch('https://localhost:7074/MainController/device');
    if (response.headers.get("content-type")?.includes("application/json")) {
        return await response.json();
    } else {
        const errorMessage = `Received non-JSON response: ${await response.text()}`;
        throw new Error(errorMessage);
    }
}
