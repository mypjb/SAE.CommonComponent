import { Button, Col, Divider, Row, Select } from 'antd';
import React, { useState } from 'react';
import { useEffect } from 'react';
import ReactJson from 'react-json-view';
import { useModel } from 'umi';
import { CloudUploadOutlined } from '@ant-design/icons'

const { Option } = Select;

export default (props) => {

    const { dispatch, id } = props;

    const environmentData = useModel("environment", model => (model.state));

    const [state, setState] = useState({ environmentId: environmentData.length ? environmentData[0].id : "", data: { preview: {}, current: {} } });

    const options = environmentData.map(data => <Option value={data.id} data={data}>{data.name}</Option>);

    useEffect(() => {
        if (state.environmentId) {
            handlerSelect(state.environmentId);
        }
    }, []);

    const handlerSelect = (environmentId) => {

        dispatch({
            type: "app/preview",
            payload: {
                data: {
                    id,
                    environmentId
                },
                callback: (data) => {
                    setState({ environmentId: environmentId, data });
                }
            }
        });
    };

    const handlerPublish = () => {
        dispatch({
            type: "app/publish",
            payload: {
                data: {
                    id: id,
                    environmentId: state.environmentId
                },
                callback: () => {
                    handlerSelect(state.environmentId);
                }
            }
        });
    }

    const jsonOption = {
        collapseStringsAfterLength: 64,
        name: false,
        displayDataTypes: false
    };

    return (
        <div>
            <Row>
                <Col span={12}>
                    <Select value={state.environmentId} style={{ width: 200 }} onSelect={handlerSelect}>
                        {options}
                    </Select>
                    <Button type="primary" onClick={handlerPublish}>
                        <CloudUploadOutlined></CloudUploadOutlined>
                    </Button>
                </Col>
                <Col span={12}>

                </Col>
            </Row>

            <Divider style={{ width: "100%" }}></Divider>
            <Row>
                <Col span={12}>
                    <h2>Preview:</h2>
                    <ReactJson src={state.data.preview || {}} {...jsonOption}></ReactJson>
                </Col>
                <Col span={12}>
                    <h2>Current:</h2>
                    <ReactJson src={state.data.current || {}} {...jsonOption}></ReactJson>
                </Col>
            </Row>

        </div>
    );
}