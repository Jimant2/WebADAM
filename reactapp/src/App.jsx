// eslint-disable-next-line no-unused-vars
import React, { Component } from 'react';

export default class App extends Component {
    static displayName = App.name;

    constructor(props) {
        super(props);
        this.state = { forecasts: [], devices: [], loadingForecasts: true, loadingDevices: true };

    }

    componentDidMount() {
        this.getDevice();
        //this.populateWeatherData();
       
    }

    static renderForecastsTable(forecasts) {
        return (
            <table className='table table-striped' aria-labelledby="tabelLabel">
                <thead>
                    <tr>
                        <th>Date</th>
                        <th>Temp. (C)</th>
                        <th>Temp. (F)</th>
                        <th>Summary</th>
                    </tr>
                </thead>
                <tbody>
                    {forecasts.map(forecast =>
                        <tr key={forecast.date}>
                            <td>{forecast.date}</td>
                            <td>{forecast.temperatureC}</td>
                            <td>{forecast.temperatureF}</td>
                            <td>{forecast.summary}</td>
                        </tr>
                    )}
                </tbody>
            </table>
        );
    }

    static renderDevices(devices) {
       return <a>Devices: {devices.name}</a>
    }

    render() {
        //let contents = this.state.loading
        //    ? <p><em>Loading... Please refresh once the ASP.NET backend has started. See <a href="https://aka.ms/jspsintegrationreact">https://aka.ms/jspsintegrationreact</a> for more details.</em></p>
        //    : App.renderForecastsTable(this.state.forecasts);
        let deviceContent = this.state.loadingDevices
            ? <p>Loading the devices...</p>
            : App.renderDevices(this.state.devices);
        return (
            <div>
                <h1 id="tabelLabel" >Weather forecast</h1>
                <p>This component demonstrates fetching data from the server.</p>
                {deviceContent}
            </div>
        );
    }

    async populateWeatherData() {
        const response = await fetch('weatherforecast');
        const data = await response.json();
        this.setState({ forecasts: data, loading: false });
    }

    async getDevice() {
        const response = await fetch('https://localhost:7074/MainController/device');
        console.log(response);
        if (response.headers.get("content-type")?.includes("application/json")) {
            const deviceData = await response.json();
            this.setState({ devices: deviceData, loading: false });
        } else {
            console.error('Received non-JSON response:', await response.text());
        }
    }


}
