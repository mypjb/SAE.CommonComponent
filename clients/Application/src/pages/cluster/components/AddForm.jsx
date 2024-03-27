import React from 'react';
import { Form, Input } from 'antd';
import { defaultFormBuild } from '@/utils/utils';

export default (props) => {
    const [form] = Form.useForm();

    const [handleSave] = defaultFormBuild({ ...props, form, dispatchType: "cluster/add" });

    return (
        <Form form={form} size='middle' onFinish={handleSave}>
            <Form.Item name="name" label="name" rules={[{ required: true }]}>
                <Input />
            </Form.Item>
        </Form>
    );
}