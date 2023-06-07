import React from 'react';
import { Select } from 'antd';
import httpClient from '../utils/service.cluster.js'

export default (props) => {

    let clusters = [];
    
    httpClient.list().then(r => {
        console.log(r);
        clusters = r;
    });

    const clusterOptions = clusters.map((cluster) => ({ label: cluster.name, value: cluster.id }));
    const defaultClusterId = clusters.length ? clusters[0].id : "";

    return (
        <Select options={clusterOptions}
            defaultValue={defaultClusterId}>
        </Select>
    );
}