import React from 'react';
import { Form, Input } from 'antd';
import { defaultFormBuild } from '@/utils/utils';

export default props => {

    console.log(props);
    const { clusterId } = props;

    const [form] = Form.useForm();

    const [handleSave] = defaultFormBuild({ ...props, form, dispatchType: "app/add" });
    
    const defaultModel = {
        clusterId
    };

    return (
        <Form form={form} size='middl' onFinish={handleSave} initialValues={defaultModel}>
            <Form.Item name="name" label="name" rules={[{ required: true }]}>
                <Input />
            </Form.Item>
            <Form.Item name="clusterId" label="clusterId" hidden rules={[{ required: true }]}>
                <Input />
            </Form.Item>
        </Form>
    );
}