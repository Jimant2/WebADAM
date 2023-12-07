// GraphVisualization.jsx
// eslint-disable-next-line no-unused-vars
import React from 'react';
import PropTypes from 'prop-types';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer, Brush } from 'recharts';
import './GraphVisualization.css';

const GraphVisualization = ({ data, dataType, formatTimestampToTime, lineColor}) => {
    console.log("Received Data:", data, "DataType:", dataType);
    console.log("Received lineColor: ", lineColor);

    const CustomTooltip = ({ active, payload, label }) => {
        if (active && payload && payload.length) {
            return (
                <div className="custom-tooltip">
                    <p className="label custom-timestamp">{`Timestamp: ${formatTimestampToTime(label)}`}</p>
                    <p className="value">{`${payload[0].name} : ${payload[0].value}`}</p>
                </div>
            );
        }
        return null;
    };
    CustomTooltip.propTypes = {
        active: PropTypes.bool,
        payload: PropTypes.array,
        label: PropTypes.number
    };
    return (
        <ResponsiveContainer className="responsive-container">
            <LineChart data={data} margin={{ top: 5, right: 30, left: 20, bottom: 5 }}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="timestamp" scale="time" type="number" domain={['auto', 'auto']} tickFormatter={formatTimestampToTime} />
                <YAxis />
                <Tooltip content={<CustomTooltip />} />
                <Legend />
                <Line type="monotone" dataKey={dataType} stroke={ lineColor } activeDot={{ r: 8 }} />
                <Brush dataKey="timestamp" height={30} stroke="#8884d8" tickFormatter={formatTimestampToTime} />
            </LineChart>
        </ResponsiveContainer>
    );
};

GraphVisualization.propTypes = {
    data: PropTypes.array.isRequired,
    dataType: PropTypes.string,
    formatTimestampToTime: PropTypes.func.isRequired,
    lineColor: PropTypes.string,
};



export default GraphVisualization;
