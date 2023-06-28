import { useState, useEffect } from 'react';
import { Cascader } from 'antd';
import httpClient from '../utils/service.cluster.js'
import { useModel } from 'umi';
import { clusterManager } from '../../config/appConfig';

export default (props) => {

    const { initialState } = useModel("@@initialState");
    const { selectHandler } = props;

    const [options, setOptions] = useState([]);

    const [selectValues, setSelectValues] = useState([]);

    useEffect(() => {
        const effectHandler = async () => {
            const clusters = (await httpClient.list()) || [];
            const clusterList = clusters.map((item) => ({ label: item.name, value: item.id, isLeaf: false }));

            setOptions(clusterList);

            const values = clusterManager.get(initialState.userInfo);

            if (values.length && clusterList.length) {

                const index = clusterList.findIndex((v) => {
                    return v.value == values[0];
                });

                if (index != -1) {
                    await loadData([clusterList[index]], clusterList);
                    setSelectValues(values);
                }
            }
        }
        effectHandler();
    }, []);

    const loadData = async (values, selectedOptions) => {
        const targetOption = values[values.length - 1];

        if (targetOption.children && targetOption.children.length) {
            return;
        }

        const clusterApps = (await httpClient.app.list(targetOption.value));

        targetOption.children = clusterApps.map((app) => ({ label: app.name, value: app.id }));

        setOptions([...(selectedOptions || options)]);
    }

    const changeHandler = (values, selectedOptions) => {
        setSelectValues(values);
        clusterManager.set(values, initialState.userInfo);
        if (values && values.length > 1)
            selectHandler(values);
    }

    return (
        <Cascader options={options}
            loadData={loadData}
            onChange={changeHandler}
            value={selectValues}
            changeOnSelect></Cascader>
    );
}