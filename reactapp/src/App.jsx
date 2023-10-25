// eslint-disable-next-line no-unused-vars
import React, { Component, useEffect } from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import { useNavigate } from 'react-router-dom';
import LoginPage from './LoginPage';
import GraphVisualization from './Component/GraphVisualization';
import { fetchDatasetsForDevice, fetchGroupsAndChannels, getAllDevices, getDeviceByName } from './Controller/APIController';
import Header from './Component/Header';
import Footer from './Component/Footer';
import './App.css';

export function AppWrapper() {
    const navigate = useNavigate();

    useEffect(() => {
        if (document.cookie.includes('CookieAuth')) {
            navigate('/main');
        }
    }, []);

    return <App />;
}

class App extends Component {
    static displayName = App.name;

    state = {
        datasets: [{ data: [], dataType: '' }, { data: [], dataType: '' }],
        devices: [],
        loadingDevices: false,
        loadingDatasets: false,
        aggregationInterval: 60 * 1000,
        groupsAndChannels: [],
        selectedDeviceName: null,
    };

    renderDevices = (devices) => {
        return <span>Device: {devices[0]?.deviceName || 'None'}</span>
    }
    renderGroupTree = () => {
        const currentDeviceName = this.state.selectedDeviceName;  // Use the state's device name
        console.log("currentDeviceName: ", currentDeviceName);

        const lookupNameById = (id) => {

    const currentDevice = this.state.devices.find(device => device.deviceName === currentDeviceName);

    if (!currentDevice || 
        !currentDevice.channelXml || 
        !currentDevice.channelXml.channelDefinition || 
        !currentDevice.channelXml.channelDefinition.channels) 
        return `Unnamed Channel ${id}`;

    // First, try to find the id in numericChannels
    let matchedChannel = currentDevice.channelXml.channelDefinition.channels.numericChannels.find(channel => channel.id === id);

    // If not found in numericChannels, try textChannels
    if (!matchedChannel) {
        matchedChannel = currentDevice.channelXml.channelDefinition.channels.textChannels.find(channel => channel.id === id);
    }

    return matchedChannel ? matchedChannel.name : `Unnamed Channel ${id}`;
};

        return (
            <div className="group-tree">
                {this.state.groupsAndChannels.map(group => (
                    <div key={group.groupName} className="group">
                        <span onClick={() => this.toggleChannelsDisplay(group.groupName)}>
                            {group.groupName}
                        </span>
                        {group.isExpanded && (
                            <ul>
                                {group.channels.map((channel, index) => (
                                    <li key={`${channel.id}-${index}`} draggable="true">
                                        {lookupNameById(channel.id)}
                                    </li>
                                ))}
                            </ul>
                        )}
                    </div>
                ))}
            </div>
        );
    }

    toggleChannelsDisplay = groupName => {
        const { groupsAndChannels } = this.state;
        const group = groupsAndChannels.find(g => g.groupName === groupName);
        group.isExpanded = !group.isExpanded;
        this.setState({ groupsAndChannels: [...groupsAndChannels] });
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
    handleDeviceSelection = async (deviceName) => {
        console.log("Device selected App.jsx:", deviceName);

        if (deviceName) {
            try {
                const deviceFromAPI = await getDeviceByName(deviceName);

                if (deviceFromAPI && !this.state.devices.some(device => device.deviceName === deviceName)) {
                    this.setState(prevState => ({
                        devices: [...prevState.devices, deviceFromAPI],
                        selectedDeviceName: deviceName
                    }));

                    fetchGroupsAndChannels(deviceName)
                        .then(response => {
                            const data = response.map(group => ({
                                ...group,
                                isExpanded: false // initialize with expanded state
                            })) || [];
                            console.log('Complete API response:', response);
                            this.setState({ groupsAndChannels: data });
                        })
                        .catch(error => {
                            console.error('Failed to fetch data:', error);
                        });
                }
            } catch (error) {
                console.error("Failed to retrieve the device:", error);
            }
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

    async fetchAllDevices() {
        try {
            const devices = await getAllDevices();
            this.setState({ devices });
            console.log("FetchAllDevicesState: " + devices);
        } catch (error) {
            console.error('Failed to fetch devices:', error);
        }
    }

    renderMainApp = () => {
        const { devices, loadingDevices, datasets } = this.state;

        let deviceContent = loadingDevices ? <p>Loading the devices...</p> : this.renderDevices(devices);

        return (
            <div className="app-container">
                <Header onDeviceSelect={this.handleDeviceSelection} />
                <div className="content-container">
                    <div className="devices-container">
                        {this.renderGroupTree()}
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

    render() {
        return (
            <Routes>
                <Route path="/" element={<LoginPage />} />
                <Route path="/main" element={this.renderMainApp()} />
            </Routes>
        );
    }

}
function Main() {
    return (
        <Router>
            <AppWrapper />
        </Router>
    );
}

export default Main;
