import React from 'react';
import { Form, Input, Select } from 'antd';
import { defaultFormBuild } from '@/utils/utils';

export default (props) => {

    const [form] = Form.useForm();

    const [handleSave] = defaultFormBuild({ ...props, form, dispatchType: "role/add" });


    return (<Form form={form} onFinish={handleSave} size='middle' >
        <Form.Item name="name" label="name" rules={[{ required: true }]}>
            <Input />
        </Form.Item>
        <Form.Item name="description" label="description" rules={[{ required: true }]}>
            <Input />
        </Form.Item>
    </Form >
    );
};