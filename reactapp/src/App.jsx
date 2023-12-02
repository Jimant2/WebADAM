// eslint-disable-next-line no-unused-vars
import React, { Component, useEffect } from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import { useNavigate } from 'react-router-dom';
import LoginPage from './LoginPage';
import GraphVisualization from './Component/GraphVisualization';
import {
    fetchDatasetsByDataType, fetchGroupsAndChannels, getAllDevices, getDeviceByName, fetchTimestampsByDataType
    , fetchDatasetsByTimestamp } from './Controller/APIController';
import Header from './Component/Header';
import Footer from './Component/Footer';
import './App.css';
import { BiAtom } from 'react-icons/bi';
import TimestampModal from './Component/TimestampsModal';

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
        datasets: [{ data: [], dataType: '', lineColor: null}, { data: [], dataType: '', lineColor: null }],
        devices: [],
        loadingDevices: false,
        loadingDatasets: false,
        aggregationInterval: 60 * 1000,
        groupsAndChannels: [],
        selectedDeviceName: null,
        valueType: '',
        dataType: '',
        selectedChannelId: '',
        isTimestampModalOpen: false,
        timestampModalData: [],
    };

    lookupNameByChannelId = (id) => {
        const currentDevice = this.state.devices.find(device => device.deviceName === this.state.selectedDeviceName);

        if (!currentDevice ||
            !currentDevice.channelXml ||
            !currentDevice.channelXml.channelDefinition ||
            !currentDevice.channelXml.channelDefinition.channels)
            return `Unnamed Channel ${id}`;

        let matchedChannel = currentDevice.channelXml.channelDefinition.channels.numericChannels.find(channel => channel.id === id);

        if (!matchedChannel) {
            matchedChannel = currentDevice.channelXml.channelDefinition.channels.textChannels.find(channel => channel.id === id);
        }
        
        return matchedChannel ? matchedChannel.name : `Unnamed Channel ${id}`;
    };
    getChannelColorById = () => {
        const { selectedChannelId, devices, selectedDeviceName } = this.state;

        const currentDevice = devices.find(device => device.deviceName === selectedDeviceName);

        if (!currentDevice ||
            !currentDevice.channelXml ||
            !currentDevice.channelXml.channelDefinition ||
            !currentDevice.channelXml.channelDefinition.channels)
            return '#8884d8'; 

        const matchedChannel = currentDevice.channelXml.channelDefinition.channels.numericChannels.find(channel => channel.id === selectedChannelId);

        return matchedChannel ? matchedChannel.color : '#8884d8';
    };

    rgbToHex(rgb) {
        const toHex = (value) => {
            if (typeof value !== 'undefined') {
                const hex = value.toString(16);
                return hex.length === 1 ? '0' + hex : hex;
            }
            return '00';
        };

        const red = toHex(rgb.red);
        const green = toHex(rgb.green);
        const blue = toHex(rgb.blue);

        return `#${red}${green}${blue}`;
    }

    renderDevices = (devices) => {
        return <span>Device: {devices[0]?.deviceName || 'None'}</span>
    }
    renderGroupTree = () => {
        const currentDeviceName = this.state.selectedDeviceName;
        console.log("currentDeviceName: ", currentDeviceName);

      const handleDragStart = (e, channelId) => {
            e.dataTransfer.setData("text/plain", channelId);
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
                                    <li key={`${channel.id}-${index}`} draggable="true" onDragStart={(e) => handleDragStart(e, channel.id)}>
                                        <BiAtom /> {this.lookupNameByChannelId(channel.id)}
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

  
    //handleDropOnGraph = (index) => async (e) => {
    //    e.preventDefault();
    //    const channelId = parseInt(e.dataTransfer.getData("text/plain"), 10);
    //    this.setState({ selectedChannelId: channelId })
    //    try {
    //        const channelName = this.lookupNameByChannelId(channelId);
    //        console.log(`Fetching data for graph ${index + 1}, channelName is:`, channelName);

    //        this.setState({ loadingDatasets: true });

    //        const { data, dataType } = await fetchDatasetsByDataType(channelName);
    //        console.log('Fetched data:', data);

    //        const interval = this.state.aggregationInterval;
    //        const aggregatedData = [];
    //        const dataByInterval = {};

    //        data.forEach(point => {
    //            const intervalKey = Math.floor(point.timestamp / interval) * interval;

    //            if (!dataByInterval[intervalKey]) {
    //                dataByInterval[intervalKey] = [];
    //            }
    //            dataByInterval[intervalKey].push(point);
    //        });

    //        for (const intervalStart in dataByInterval) {
    //            const points = dataByInterval[intervalStart];
    //            const avgValue = Math.round(points.reduce((sum, point) => sum + point[dataType], 0) / points.length);
    //            aggregatedData.push({ timestamp: Number(intervalStart), [dataType]: avgValue });
    //        }

    //        console.log("Aggregated Data:", aggregatedData);

    //        this.setState(prevState => {
    //            const newDatasets = [...prevState.datasets];
    //            newDatasets[index] = { data: aggregatedData, dataType: dataType, lineColor: this.rgbToHex(this.getChannelColorById(index))};
    //            return { datasets: newDatasets, loadingDatasets: false };
    //        });
    //    } catch (error) {
    //        console.error(error);
    //        this.setState({ loadingDatasets: false });
    //    }
    //}

    handleDropOnGraph = (index) => async (e) => {
        e.preventDefault();
        const channelId = parseInt(e.dataTransfer.getData("text/plain"), 10);
        this.setState({ selectedChannelId: channelId });

        try {
            const channelName = this.lookupNameByChannelId(channelId);
            console.log(`Fetching timestamps for graph ${index + 1}, channelName is:`, channelName);

            // Fetch timestamps for the selected channelName
            const timestamps = await fetchTimestampsByDataType(channelName);
            console.log('Fetched timestamps:', timestamps);

            this.setState({
                isTimestampModalOpen: true,
                timestampModalData: timestamps,
                loadingDatasets: false,
                selectedTimestamp: null, // Reset selectedTimestamp
            });
        } catch (error) {
            console.error(error);
            this.setState({ loadingDatasets: false });
        }
    };


    handleTimestampSelect = (index) => async (selectedTimestamp, newWindow) => {
        try {
            this.setState({ isTimestampModalOpen: false, loadingDatasets: true });

            // Fetch datasets based on the selected timestamp
            const { data, dataType } = await fetchDatasetsByTimestamp(selectedTimestamp);

            // Process the fetched datasets as needed
            const interval = this.state.aggregationInterval;
            const aggregatedData = [];
            const dataByInterval = {};

            data.forEach(point => {
                const intervalKey = Math.floor(point.timestamp / interval) * interval;

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

            // Close the new window
            newWindow.close();

            this.setState(prevState => {
                const newDatasets = [...prevState.datasets];
                newDatasets[index] = { data: aggregatedData, dataType: dataType, lineColor: this.rgbToHex(this.getChannelColorById(index)) };
                return { datasets: newDatasets, loadingDatasets: false };
            });
        } catch (error) {
            console.error(error);
            this.setState({ loadingDatasets: false });
        }
    };

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
                                isExpanded: false
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

    fetchDatasetsAndSetState = async (dataType) => {
        try {
            this.setState({ loadingDatasets: true });
            const dataSets = await fetchDatasetsByDataType(dataType);
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
    handleLogout = () => {
        this.setState({
            datasets: [{ data: [], dataType: '', lineColor: null }, { data: [], dataType: '', lineColor: null }],
            devices: [],
            loadingDevices: false,
            loadingDatasets: false,
            aggregationInterval: 60 * 1000,
            groupsAndChannels: [],
            selectedDeviceName: null,
            valueType: '',
            dataType: '',
            selectedChannelId: ''
        });
    };
    renderMainApp = () => {
        const { devices, loadingDevices, datasets } = this.state;


        let deviceContent = loadingDevices ? <p>Loading the devices...</p> : this.renderDevices(devices);

        return (
            <div className="app-container">
                <Header onDeviceSelect={this.handleDeviceSelection} onLogout={this.handleLogout}/>
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
                        <TimestampModal
                            isOpen={this.state.isTimestampModalOpen}
                            onClose={() => this.setState({ isTimestampModalOpen: false })}
                            timestamps={this.state.timestampModalData}
                            onSelectTimestamp={this.handleTimestampSelect}
                        />
                        <div className="graph"
                            onDrop={this.handleDropOnGraph(0)} 
                            onDragOver={(e) => e.preventDefault()}>
                            <GraphVisualization data={datasets[0].data} formatTimestampToTime={this.formatTimestampToTime} dataType={datasets[0].dataType} lineColor={this.state.datasets[0].lineColor} />
                        </div>
                        <div className="graph"
                            onDrop={this.handleDropOnGraph(1)} 
                            onDragOver={(e) => e.preventDefault()}>
                            <GraphVisualization data={datasets[1].data} formatTimestampToTime={this.formatTimestampToTime} dataType={datasets[1].dataType} lineColor={this.state.datasets[1].lineColor} />
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
