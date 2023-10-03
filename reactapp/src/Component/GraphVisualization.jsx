// eslint-disable-next-line no-unused-vars
import React from 'react';
import PropTypes from 'prop-types';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';

const GraphVisualization = ({ data, dataType, formatTimestampToTime }) => {
    console.log("Received Data:", data, "DataType:", dataType);
    return (
        <ResponsiveContainer>
            <LineChart data={data} margin={{ top: 5, right: 30, left: 20, bottom: 5 }}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="timestamp" scale="time" type="number" domain={['auto', 'auto']} tickFormatter={formatTimestampToTime} />
                <YAxis />
                <Tooltip />
                <Legend />
                <Line type="monotone" dataKey={dataType} stroke="#8884d8" activeDot={{ r: 8 }} />
            </LineChart>
        </ResponsiveContainer>
    );
};


GraphVisualization.propTypes = {
    data: PropTypes.array.isRequired,
    dataType: PropTypes.string,
    formatTimestampToTime: PropTypes.func.isRequired,
};



export default GraphVisualization;
