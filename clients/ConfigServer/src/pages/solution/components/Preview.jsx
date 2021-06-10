import { Col, Divider, Row, Select } from 'antd';
import React, { useState } from 'react';
import ReactJson from 'react-json-view';
import { useModel } from 'umi';

const { Option } = Select;

export default (props) => {

    const { dispatch, id } = props;

    const environments = useModel("environment", model => (model.state));

    const [state, setState] = useState({ preview: {}, current: {} });

    const options = environments.map(data => <Option value={data.id} data={data}>{data.name}</Option>);

    const handlerSelect = (option) => {
        const { value, label } = option;
        dispatch({
            type: "project/preview",
            payload: {
                data: {
                    id,
                    environmentId: value,
                    env: label
                },
                callback: (model) => {
                    setState(model);
                }
            }
        });
    };

    return (
        <div>
            <Select labelInValue style={{ width: 200 }} onSelect={handlerSelect}>
                {options}
            </Select>
            <Row>
                <Col span={12}>Preview</Col>
                <Col span={12}>Current</Col>
            </Row>
            <Divider style={{ width: "100%" }}></Divider>
            <Row>
                <Col span={12}>
                    <ReactJson src={state.preview} collapseStringsAfterLength={64} name={false} displayDataTypes={false}></ReactJson>

                </Col>
                <Col span={12}>
                    <ReactJson src={state.current} name={false} displayDataTypes={false}></ReactJson>
                </Col>
            </Row>

        </div>
    );
}