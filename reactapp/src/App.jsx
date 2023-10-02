// eslint-disable-next-line no-unused-vars
import React, { Component } from 'react';
import GraphVisualization from './Component/GraphVisualization';
import { fetchDatasetsForDevice, getDevice } from './Controller/APIController';
import Header from './Component/Header';
import Footer from './Component/Footer';
import './App.css';

export default class App extends Component {
    static displayName = App.name;

    state = {
        datasets: [[], []],
        devices: [],
        loadingDevices: true,
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
    handleDropOnGraph = (index) => async (e) => {
        e.preventDefault();
        const deviceName = e.dataTransfer.getData("text/plain");
        console.log(`Fetching data for graph ${index + 1}, device:`, deviceName);
        try {
            this.setState({ loadingDatasets: true });
            const dataSets = await fetchDatasetsForDevice(deviceName);
            console.log('Fetched data:', dataSets);
            // Check if dataSets.Data is defined and is an array before calling .map()
            const transformedData = Array.isArray(dataSets.Data) ? dataSets.Data.map(dataPoint => ({
                ...dataPoint,
                dataType: dataSets.dataType,
            })) : [];

            this.setState(prevState => {
                const newDatasets = [...prevState.datasets];
                newDatasets[index] = transformedData;
                return { datasets: newDatasets, loadingDatasets: false };
            });
        } catch (error) {
            console.error(error);
            this.setState({ loadingDatasets: false });
        }
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
        const { devices, loadingDevices, datasets } = this.state;

        let deviceContent = loadingDevices ? <p>Loading the devices...</p> : this.renderDevices(devices);

        return (
            <div className="app-container">
                <Header />
                <div className="content-container">
                    <div className="devices-container">
                        {devices.map(device => (
                            <div key={device.id}
                                className="device"
                                draggable
                                onDragStart={(e) => e.dataTransfer.setData("text/plain", device.deviceName)}>
                                {device.deviceName}
                            </div>
                        ))}
                    </div>
                    <div className="graphs-container">
                        {deviceContent}
                        <div className="graph"
                            onDrop={this.handleDropOnGraph(0)} // Pass the index here
                            onDragOver={(e) => e.preventDefault()}>
                            <GraphVisualization data={datasets[0]} formatTimestampToTime={this.formatTimestampToTime} dataType="dataType" />
                        </div>
                        <div className="graph"
                            onDrop={this.handleDropOnGraph(1)} // Pass the index here
                            onDragOver={(e) => e.preventDefault()}>
                            <GraphVisualization data={datasets[1]} formatTimestampToTime={this.formatTimestampToTime} dataType="dataType" />
                        </div>
                    </div>
                </div>
                <Footer />
            </div>
        );
        }

    }


