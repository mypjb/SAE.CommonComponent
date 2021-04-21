import React from 'react';
import { Form, Input, Modal } from 'antd';

export default (props) => {
    const { dispatch, model, callback } = props;
    const [form] = Form.useForm();
    const handleSave = (data) => {
        dispatch({ type: 'projectConfig/edit', payload: { data, callback: props.close } });
    }

    const handleOk = () => {
        form.submit();
        return false;
    };

    callback(handleOk);

    form.setFieldsValue(model);

    return (<Form form={form} size='middl' onFinish={handleSave} >
        <Form.Item name="alias" label="alias" rules={[{ required: true }]}>
            <Input />
        </Form.Item>
        <Form.Item name="id" style={{ display: "none" }}>
            <Input />
        </Form.Item>
    </Form>);

};