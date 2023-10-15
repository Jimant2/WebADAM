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
        datasets: [{ data: [], dataType: '' }, { data: [], dataType: '' }],
        devices: [],
        loadingDevices: true,
        loadingDatasets: false,
        aggregationInterval: 60 * 1000,
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
            const { data, dataType } = await fetchDatasetsForDevice(deviceName);
            console.log('Fetched data:', data);

            // Aggregate the data based on the selected interval
            const interval = this.state.aggregationInterval;
            const aggregatedData = [];
            const dataByInterval = {};

            data.forEach(point => {
                const intervalKey = Math.floor(point.timestamp / interval) * interval; // Group by the selected interval

                if (!dataByInterval[intervalKey]) {
                    dataByInterval[intervalKey] = [];
                }
                dataByInterval[intervalKey].push(point);
            });

            for (const intervalStart in dataByInterval) {
                const points = dataByInterval[intervalStart];
                const avgValue = Math.round(points.reduce((sum, point) => sum + point[dataType], 0) / points.length);
                aggregatedData.push({ timestamp: Number(intervalStart), [dataType]: avgValue });
            }

            console.log("Aggregated Data:", aggregatedData);

            this.setState(prevState => {
                const newDatasets = [...prevState.datasets];
                newDatasets[index] = { data: aggregatedData, dataType: dataType };
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
                        <div className="aggregation-dropdown">
                            <label>Interval: </label>
                            <select
                                value={this.state.aggregationInterval}
                                onChange={(e) => this.setState({ aggregationInterval: Number(e.target.value) })}
                            >
                                <option value={0.5 * 1000}>0.5 seconds</option>
                                <option value={1 * 1000}>1 seconds</option>
                                <option value={5 * 1000}>5 seconds</option>
                                <option value={30 * 1000}>30 seconds</option>
                                <option value={60 * 1000}>1 minute</option>
                                {/* Add more options as needed */}
                            </select>
                        </div>
                        <div className="graph"
                            onDrop={this.handleDropOnGraph(0)} // Pass the index here
                            onDragOver={(e) => e.preventDefault()}>
                            <GraphVisualization data={datasets[0].data} formatTimestampToTime={this.formatTimestampToTime} dataType={datasets[0].dataType || "value"} />
                        </div>
                        <div className="graph"
                            onDrop={this.handleDropOnGraph(1)} // Pass the index here
                            onDragOver={(e) => e.preventDefault()}>
                            <GraphVisualization data={datasets[1].data} formatTimestampToTime={this.formatTimestampToTime} dataType={datasets[1].dataType || "value"} />
                        </div>
                    </div>
                    
                </div>
                <Footer />
            </div>
        );
        }

    }


