import React from 'react';
import { Form, Input, Modal } from 'antd';
import { defaultFormBuild } from '@/utils/utils';

export default props => {

    const { model } = props;
    const [form] = Form.useForm();

    form.setFieldsValue(model);

    const [handleSave] = defaultFormBuild({ ...props, form, type: "environment/edit" });

    return (<Form form={form} size='middl' onFinish={handleSave} >
        <Form.Item name="name" label="name" rules={[{ required: true }]}>
            <Input />
        </Form.Item>
        <Form.Item name="id" hidden>
            <Input/>
        </Form.Item>
    </Form>);

}