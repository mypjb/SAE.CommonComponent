import { Select } from 'antd';
import React, { useState } from 'react';
import ReactJson from 'react-json-view';
import { useModel } from 'umi';

const { Option } = Select;

export default (props) => {

    const { dispatch, id } = props;

    const environments = useModel("environment", model => (model.state));
    const environmentId = environments.length ? environments[0].id : null;
    const [state, setState] = useState();

    const options = environments.map(data => <Option value={data.id} data={data}>{data.name}</Option>);

    const handlerSelect = (envId) => {
        dispatch({
            type: "project/preview",
            payload: {
                data: {
                    id,
                    environmentId: envId
                },
                callback: (model) => {
                    setState(model);
                }
            }
        });
    };

    return (
        <div>
            <Select defaultValue={environmentId} onSelect={handlerSelect}>
                {options}
            </Select>
            <ReactJson src={state}></ReactJson>
        </div>
    );
}