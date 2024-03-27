import React from 'react';
import { Form, Input, Modal } from 'antd';
import { defaultFormBuild } from '@/utils/utils';

export default props => {

    const { model } = props;
    const [form] = Form.useForm();

    form.setFieldsValue(model);

    const [handleSave] = defaultFormBuild({ ...props, form, dispatchType: "cluster/edit" });

    return (<Form form={form} size='middle' onFinish={handleSave} >
        <Form.Item name="name" label="name" rules={[{ required: true }]}>
            <Input />
        </Form.Item>
        <Form.Item name="id" style={{ display: "none" }}>
            <Input />
        </Form.Item>
    </Form>);

}