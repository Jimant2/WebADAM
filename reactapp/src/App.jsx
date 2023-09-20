// eslint-disable-next-line no-unused-vars
import React, { Component } from 'react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';

export default class App extends Component {
    static displayName = App.name;

    state = {
        devices: [],
        loadingDevices: true,
        currentDatasets: [],
        loadingDatasets: false,
    };

    componentDidMount() {
        this.getDevice();
    }

    renderDevices = (devices) => {
        return <span>Devices: {devices[0]?.deviceName || 'None'}</span>
    }
    formatTimestampToTime = (timestamp) => {
        const date = new Date(timestamp);
        return `${date.getHours().toString().padStart(2, '0')}:${date.getMinutes().toString().padStart(2, '0')}:${date.getSeconds().toString().padStart(2, '0')}`;
    }
    handleDropOnGraph = async (e) => {
        e.preventDefault();
        const deviceName = e.dataTransfer.getData("text/plain");
        console.log("Fetching data for device:", deviceName);
        await this.fetchDatasetsForDevice(deviceName);
    }

    fetchDatasetsForDevice = async (deviceName) => {
        this.setState({ loadingDatasets: true });
        const response = await fetch(`https://localhost:7074/MainController/dataSetByName/${deviceName}`);
        if (response.ok) {
            const dataSets = await response.json();
            const mappedData = dataSets.flatMap(dataset => (
                dataset.data.map(dataPoint => ({
                    timestamp: new Date(dataPoint.timestamp).getTime(),
                    value: dataPoint.value
                }))
            ));
            mappedData.sort((a, b) => a.timestamp - b.timestamp);
            this.setState({ currentDatasets: mappedData, loadingDatasets: false });
        } else {
            console.error('Failed to fetch datasets:', await response.text());
            this.setState({ loadingDatasets: false });
        }
    }




    getDevice = async () => {
        const response = await fetch('https://localhost:7074/MainController/device');
        if (response.headers.get("content-type")?.includes("application/json")) {
            const deviceData = await response.json();
            this.setState({ devices: deviceData, loadingDevices: false });
        } else {
            console.error('Received non-JSON response:', await response.text());
        }
    }

    render() {
        const { devices, loadingDevices, currentDatasets } = this.state;

        let deviceContent = loadingDevices ? <p>Loading the devices...</p> : this.renderDevices(devices);

        return (
            <div style={{ display: 'flex' }}>
                <div>
                    {devices.map(device => (
                        <div key={device.id}
                            draggable
                            onDragStart={(e) => e.dataTransfer.setData("text/plain", device.deviceName)}>
                            {device.deviceName}
                        </div>
                    ))}
                </div>

                <div style={{ flex: 1, marginLeft: '20px' }}>
                    <p>This component demonstrates fetching data from the server.</p>
                    {deviceContent}

                    <div
                        onDrop={this.handleDropOnGraph}
                        onDragOver={(e) => e.preventDefault()}
                        style={{ width: "100%", height: 300 }}>
                        <ResponsiveContainer>
                            <LineChart data={currentDatasets}
                                margin={{ top: 5, right: 30, left: 20, bottom: 5 }}>
                                <CartesianGrid strokeDasharray="3 3" />
                                <XAxis dataKey="timestamp" scale="time" type="number" domain={['auto', 'auto']} tickFormatter={this.formatTimestampToTime} />
                                <YAxis />
                                <Tooltip />
                                <Legend />
                                <Line type="monotone" dataKey="value" stroke="#8884d8" activeDot={{ r: 8 }} />
                            </LineChart>
                        </ResponsiveContainer>
                    </div>
                </div>
            </div>
        );
    }
}


