import React from 'react';
import { Form, Input, Select } from 'antd';
import { defaultFormBuild, Format } from '@/utils/utils';

export default (props) => {

    const { model } = props;

    // const appTypes = [];
    // appTypes.push({ id: model.type, name: Format.appType(model.type) });

    const [form] = Form.useForm();

    const [handleSave] = defaultFormBuild({ ...props, form, dispatchType: "app/edit" });

    // const appOptions = appTypes.map((val) => {
    //     return (<Option value={val.id}>{val.name}</Option>);
    // });

    form.setFieldsValue(model);

    return (
        <Form form={form} onFinish={handleSave} size='middl'>
            <Form.Item name="id" label="id" rules={[{ required: true }]} hidden>
                <Input />
            </Form.Item>
            <Form.Item name="parentId" label="parent" rules={[{ required: true }]}>
                <Select disabled={true}>
                    <Option value={model.parent.id}>{model.parent.name}</Option>
                </Select>
            </Form.Item>
            <Form.Item name="name" label="name" rules={[{ required: true }]}>
                <Input />
            </Form.Item>
            {/* <Form.Item name="type" label="type" rules={[{ required: true }]}>
                <Select>
                    {appOptions}
                </Select>
            </Form.Item> */}
        </Form>
    );
};