import React from 'react';
import { Form, Input, Select } from 'antd';
import { defaultFormBuild } from '@/utils/utils';

export default (props) => {

    const { model } = props;

    const [form] = Form.useForm();

    const [handleSave] = defaultFormBuild({ ...props, form, dispatchType: "plugin/edit" });

    form.setFieldsValue(model);

    return (
        <Form form={form} onFinish={handleSave} size='middle'>
            <Form.Item name="id" label="id" rules={[{ required: true }]} hidden>
                <Input />
            </Form.Item>
            <Form.Item name="entry" label="entry" rules={[{ required: true }]} >
                <Input />
            </Form.Item>
            <Form.Item name="path" label="path" rules={[{ required: true }]} >
                <Input />
            </Form.Item>
        </Form>
    );
};