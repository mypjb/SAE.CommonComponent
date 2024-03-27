import React from 'react';
import { Form, Input, Select } from 'antd';
import { defaultFormBuild } from '@/utils/utils';

const { Option } = Select;

export default (props) => {

    const { model } = props;

    const [form] = Form.useForm();

    const [handleSave] = defaultFormBuild({ ...props, form, dispatchType: "menu/add" });

    const parentName = model && model.name ? model.name : "root";

    return (<Form form={form} onFinish={handleSave} size='middle' initialValues={{ parentName, hidden: true, parentId: model?.id }}>
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
        <Form.Item name="hidden" label="hidden" rules={[{ required: true }]}>
            <Select>
                <Option value={true}>hidden</Option>
                <Option value={false}>display</Option>
            </Select>
        </Form.Item>
    </Form >
    );
};