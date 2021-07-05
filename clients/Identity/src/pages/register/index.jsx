import { Input, Button, Form } from 'antd';
import { connect, useLocation, useModel } from 'umi';

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
    const model = useModel('@@initialState');

    console.log({ model });

    const [form] = Form.useForm();

    const handlerSubmit = (data) => {
        dispatch({
            type: "account/register",
            payload: {
                callback: () => {
                    console.log(`${data.name} register ok`);
                },
                data
            }
        });
    }

    return (
        <Form
            {...layout}
            name="basic"
            method="post"
            initialValues={{
                remember: true,
            }}
            form={form}
            onFinish={handlerSubmit}>
            <Form.Item
                label="Name"
                name="name"
                rules={[
                    {
                        required: true,
                        message: 'Please input your username!',
                    },
                    {
                        pattern: "^[a-zA-Z0-9_-]{4,16}$",
                        message: "Usernames can only be filled in with alphanumeric underscores and are 6~18 characters long"
                    }
                ]}
            >
                <Input />
            </Form.Item>

            <Form.Item
                label="Password"
                name="password"
                rules={[
                    {
                        required: true,
                        message: 'Please input your password!',
                    },
                    {
                        type: "string",
                        pattern: /^.*(?=.{6,24})(?=.*\d)(?=.*[A-Z])(?=.*[a-z])(?=.*[!@#\$%\^&\*\?]).*$/,
                        message: "Passwords must be between 6 and 24 characters long and include at least 1 uppercase, 1 lowercase, 1 numeric, and 1 special character"
                    }
                ]}
            >
                <Input.Password />
            </Form.Item>

            <Form.Item
                label="confirm password"
                name="confirmPassword"
                dependencies={['password']}
                rules={[
                    {
                        required: true,
                        message: 'Please input your confirm password!',
                    },
                    ({ getFieldValue }) => ({
                        validator(_, value) {
                            if (!value || getFieldValue('password') === value) {
                                return Promise.resolve();
                            }
                            return Promise.reject(new Error('The two passwords that you entered do not match!'));
                        },
                    }),
                ]}
            >
                <Input.Password />
            </Form.Item>


            <Form.Item {...tailLayout}>
                <Button type="primary" htmlType="submit">
                    Submit
                </Button>
            </Form.Item>
        </Form>
    );
});