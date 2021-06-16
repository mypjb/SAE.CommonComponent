import React from 'react';
import { Form, Input, Select } from 'antd';
import { defaultFormBuild } from '@/utils/utils';

export default (props) => {

    const {  model } = props;

    const [form] = Form.useForm();

    const [handleSave] = defaultFormBuild({ ...props, form, dispatchType: "menu/add" });

    const parentName = model && model.name ? model.name : "root";

    return (<Form form={form} onFinish={handleSave} size='middl' initialValues={{ parentName, parentId: model   ?.id }}>
         <Form.Item name="parentName" label="parent" rules={[{ required: true }]}>
            <Select disabled={true}></Select>
        </Form.Item>
        <Form.Item name="parentId" label="parentId" hidden>
            <Input />
        </Form.Item>
        <Form.Item name="name" label="name" rules={[{ required: true }]}>
            <Input />
        </Form.Item>
        <Form.Item name="path" label="path" rules={[{ required: true }]}>
            <Input />
        </Form.Item>
    </Form >
    );
};