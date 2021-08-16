import React from 'react';
import { Form, Input, Modal } from 'antd';
import { defaultFormBuild } from '@/utils/utils';

export default (props) => {
    const { model } = props;

    const [form] = Form.useForm();

    const [handleSave] = defaultFormBuild({ ...props, form, dispatchType: "appConfig/edit" });

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