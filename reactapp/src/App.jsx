// eslint-disable-next-line no-unused-vars
import React, { Component } from 'react';
import GraphVisualization from './Component/GraphVisualization';
import { fetchDatasetsForDevice, getDevice } from './Controller/APIController';

export default class App extends Component {
    static displayName = App.name;

    state = {
        devices: [],
        loadingDevices: true,
        currentDatasets: [],
        loadingDatasets: false,
    };

    componentDidMount() {
        this.getDeviceAndSetState();
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
        await this.fetchDatasetsAndSetState(deviceName);
    }

    fetchDatasetsAndSetState = async (deviceName) => {
        try {
            this.setState({ loadingDatasets: true });
            const dataSets = await fetchDatasetsForDevice(deviceName);
            this.setState({ currentDatasets: dataSets, loadingDatasets: false });
        } catch (error) {
            console.error(error);
            this.setState({ loadingDatasets: false });
        }
    }

    getDeviceAndSetState = async () => {
        try {
            const devices = await getDevice();
            this.setState({ devices: devices, loadingDevices: false });
        } catch (error) {
            console.error(error);
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
                        <GraphVisualization data={currentDatasets} formatTimestampToTime={this.formatTimestampToTime} />
                    </div>
                </div>
            </div>
        );
    }
}


