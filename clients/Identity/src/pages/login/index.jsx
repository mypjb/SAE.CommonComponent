import { Row, Col, Input, Table, Button, Modal, Form, Checkbox } from 'antd';
import { UserOutlined, LockOutlined } from '@ant-design/icons';
import { connect, useLocation,useModel } from 'umi';

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


export default connect()(({ dispatch }) => {
    const { initialState } = useModel('@@initialState');
    const formId="loginForm";
    const query = useLocation().query;

    const [form] = Form.useForm();

    const handlerSubmit = () => {
        document.getElementById(formId).submit();
    }
    const array=[];
    for(let key in query){
        array.push(<Input name={key} type="hidden" value={query[key]} />)
    }

    return (
        <Form
            {...layout}
            name="basic"
            method="post"
            action={initialState.login}
            id={formId}
            initialValues={{
                remember: true,
            }}
            form={form}
            onFinish={handlerSubmit}
        >
            {array}
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
});