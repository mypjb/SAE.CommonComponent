import React from 'react';
import { Form, Input, Select } from 'antd';
import { defaultFormBuild, Format } from '@/utils/utils';
import { useModel } from 'umi';

const { Option } = Select;


export default (props) => {


    const model = useModel('scope', model => ({ scope: model.state, load: model.load }));

    console.log({ model });
    const [form] = Form.useForm();

    const [handleSave] = defaultFormBuild({ ...props, form, dispatchType: "app/add" });

    // const appOptions = appTypes.map((val) => {
    //     return (<Option value={val.id}>{val.name}</Option>);
    // });

    return (<Form form={form} onFinish={handleSave} size='middl'>
        <Form.Item name="name" label="name" rules={[{ required: true }]}>
            <Input />
        </Form.Item>
    </Form >
    );
};