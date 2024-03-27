import React from 'react';
import { Form, Input, Select } from 'antd';
import { defaultFormBuild } from '@/utils/utils';

export default (props) => {

    const [form] = Form.useForm();

    const [handleSave] = defaultFormBuild({ ...props, form, dispatchType: "user/add" });


    return (<Form form={form} onFinish={handleSave} size='middle' >
        <Form.Item name="name" label="name" rules={[
            {
                required: true,
                message: 'Please input your username!',
            },
            {
                pattern: "^[a-zA-Z0-9_-]{4,16}$",
                message: "Usernames can only be filled in with alphanumeric underscores and are 6~18 characters long"
            }
        ]}>
            <Input />
        </Form.Item>
        <Form.Item name="password" label="password" rules={[
            {
                required: true,
                message: 'Please input your password!',
            },
            {
                type: "string",
                pattern: /^.*(?=.{6,24})(?=.*\d)(?=.*[A-Z])(?=.*[a-z])(?=.*[!@#\$%\^&\*\?]).*$/,
                message: "Passwords must be between 6 and 24 characters long and include at least 1 uppercase, 1 lowercase, 1 numeric, and 1 special character"
            }
        ]}>
            <Input />
        </Form.Item>
    </Form >
    );
};