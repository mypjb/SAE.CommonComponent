import React from 'react';
import { Form, Input, Select } from 'antd';
import { defaultFormBuild } from '@/utils/utils';

export default (props) => {

    const { model } = props;

    const [form] = Form.useForm();

    const [handleSave] = defaultFormBuild({ ...props, form, dispatchType: "menu/edit" });

    if (!model.parentName) {
        model.parentName = "root";
    }

    form.setFieldsValue(model);

    return (
        <Form form={form} onFinish={handleSave} size='middl'>
            <Form.Item name="parentId" label="parentId" rules={[{ required: true }]} hidden>
                <Input />
            </Form.Item>
            <Form.Item name="id" label="id" rules={[{ required: true }]} hidden>
                <Input />
            </Form.Item>
            <Form.Item name="parentName" label="parent" rules={[{ required: true }]}>
                <Select disabled={true}></Select>
            </Form.Item>
            <Form.Item name="name" label="name" rules={[{ required: true }]}>
                <Input />
            </Form.Item>
            <Form.Item name="path" label="path" rules={[{ required: true }]}>
                <Input />
            </Form.Item>
            <Form.Item name="microApp" label="microApp" >
                <Input />
            </Form.Item>
            <Form.Item name="hidden" label="hidden" rules={[{ required: true }]}>
                <Select>
                    <Option value={true}>hidden</Option>
                    <Option value={false}>display</Option>
                </Select>
            </Form.Item>
        </Form>
    );
};