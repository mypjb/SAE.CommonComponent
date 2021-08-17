import { Col, Divider, Row, Select } from 'antd';
import React, { useState } from 'react';
import { useEffect } from 'react';
import ReactJson from 'react-json-view';
import { useModel } from 'umi';

const { Option } = Select;

export default (props) => {

    const { dispatch, id } = props;

    const environmentData = useModel("environment", model => (model.state));

    const environmentId = environmentData.length ? environmentData[0].id : "";

    const [state, setState] = useState({ preview: {}, current: {} });

    const options = environmentData.map(data => <Option value={data.id} data={data}>{data.name}</Option>);

    useEffect(() => {
        if (environmentId) {
            handlerSelect(environmentId);
        }
    }, []);

    const handlerSelect = (envId) => {
        dispatch({
            type: "app/preview",
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

    const jsonOption = {
        collapseStringsAfterLength: 64,
        name: false,
        displayDataTypes: false
    };

    return (
        <div>
            <Select defaultValue={environmentId} style={{ width: 200 }} onSelect={handlerSelect}>
                {options}
            </Select>
            <Row>
                <Col span={12}>Preview</Col>
                <Col span={12}>Current</Col>
            </Row>
            <Divider style={{ width: "100%" }}></Divider>
            <Row>
                <Col span={12}>
                    <ReactJson src={state.preview || {}} {...jsonOption}></ReactJson>
                </Col>
                <Col span={12}>
                    <ReactJson src={state.current || {}} {...jsonOption}></ReactJson>
                </Col>
            </Row>

        </div>
    );
}