import { Row, Col, Input, Table, Button, Modal, Form, Checkbox } from 'antd';
import { UserOutlined, LockOutlined } from '@ant-design/icons';
import {  useLocation, useModel } from 'umi';

const layout = {
    labelCol: {
        span: 8,
    },
    wrapperCol: {
        span: 8,
    },
};
const tailLayout = {
    wrapperCol: {
        offset: 8,
        span: 16,
    },
};

export default () => {

    const { initialState } = useModel('@@initialState');

    const { login } = initialState.masterProps.masterState.siteConfig.api;

    const formId = "loginForm";
    const location = useLocation();

    const [form] = Form.useForm();

    const handlerSubmit = () => {
        document.getElementById(formId).submit();
    }

    let loginUrl = login;

    if (location.search) {
        loginUrl = loginUrl + (loginUrl.indexOf('?') ? "" : "&") + location.search;
    }

    console.log({ loginUrl, login });

    return (
        <Form
            {...layout}
            name="basic"
            method="post"
            action={loginUrl}
            id={formId}
            initialValues={{
                remember: true,
            }}
            form={form}
            onFinish={handlerSubmit}
        >

            <Form.Item
                label="Name"
                name="name"
                rules={[
                    {
                        required: true,
                        message: 'Please input your username!',
                    },
                ]}
            >
                <Input name="name" />
            </Form.Item>

            <Form.Item
                label="Password"
                name="password"
                rules={[
                    {
                        required: true,
                        message: 'Please input your password!',
                    },
                ]}
            >
                <Input.Password name="password" />
            </Form.Item>

            <Form.Item {...tailLayout} name="remember" valuePropName="checked">
                <Checkbox name="remember" value="true">Remember me</Checkbox>
            </Form.Item>

            <Form.Item {...tailLayout}>
                <Button type="primary" htmlType="submit">
                    Submit
                </Button>
            </Form.Item>
        </Form>
    );
};