import React from 'react';
import { Form, Input } from 'antd';
import { defaultFormBuild } from '@/utils/utils';

export default props => {

    const { solutionId } = props;

    const [form] = Form.useForm();

    const [handleSave] = defaultFormBuild({ ...props, form, dispatchType: "project/add" });

    return (
        <Form form={form} size='middl' onFinish={handleSave}>
            <Form.Item name="name" label="name" rules={[{ required: true }]}>
                <Input />
            </Form.Item>
            <Form.Item name="solutionId" label="solutionId" hidden initialValue={solutionId} rules={[{ required: true }]}>
                <Input />
            </Form.Item>
        </Form>
    );
}