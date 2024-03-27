import React from 'react';
import { Form, Input } from 'antd';
import { defaultFormBuild } from '@/utils/utils';
import { validatorJson, handleFormat } from '@/utils/utils.extension';

const { TextArea } = Input;

export default (props) => {

    const { model } = props;

    const [form] = Form.useForm();

    const [handleSave] = defaultFormBuild({ ...props, form, dispatchType: "template/edit" });

    form.setFieldsValue(model);

    const handleFormatFormat = handleFormat.bind(this, { form, fieldName: 'format' });

    return (<Form form={form} size='middle' onFinish={handleSave} >
        <Form.Item name="id" style={{ display: "none" }}>
            <Input />
        </Form.Item>
        <Form.Item name="name" label="name" rules={[{ required: true }]}>
            <Input />
        </Form.Item>
        <Form.Item name="format" label="format" rules={[{ validator: validatorJson, required: true }]}>
            <TextArea autoSize={{ minRows: 16 }} onDoubleClick={handleFormatFormat} />
        </Form.Item>
    </Form>);

};