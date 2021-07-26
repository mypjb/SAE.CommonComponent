import React from 'react';
import { Button, Form, Input, Select } from 'antd';
import { defaultFormBuild, guid } from '@/utils/utils';
import { useModel } from 'umi';
import { MinusCircleOutlined, PlusCircleOutlined } from '@ant-design/icons';
const { Option } = Select;


export default (props) => {

    const { scope } = useModel('scope', model => ({ scope: model.state, load: model.load }));

    const [form] = Form.useForm();

    const [handleSave] = defaultFormBuild({ ...props, form, dispatchType: "app/add" });

    const scopeOptions = scope.map((val) => {
        return (<Option value={val.id}>{val.name}</Option>);
    });


    const defaultModel = {
        id: guid(),
        secret: guid(),
        redirectUris: ["1", "2"]
    };


    const formItemLayoutWithOutLabel = {
        wrapperCol: {
            xs: { span: 24, offset: 0 },
            sm: { span: 20, offset: 4 },
        },
    };

    return (<Form form={form} onFinish={handleSave} size='middl' initialValues={defaultModel}>
        <Form.Item name="id" label="id" rules={[{ required: true }]}>
            <Input />
        </Form.Item>
        <Form.Item name="secret" label="secret" rules={[{ required: true }]}>
            <Input />
        </Form.Item>
        <Form.Item name="name" label="name" rules={[{ required: true }]}>
            <Input />
        </Form.Item>
        <Form.Item name="scopes" label="scope" rules={[{ required: true }]}>
            <Select mode="multiple"
                placeholder="select scope">
                {scopeOptions}
            </Select>
        </Form.Item>
        <Form.List name={['endpoint', 'redirectUris']} rules={[
            {
                validator: async (_, redirectUris) => {
                    if (!redirectUris || redirectUris.length < 1) {
                        return Promise.reject(new Error('At least 1 passengers'));
                    }
                },
            },
        ]}>

            {(fields, { add, remove }, { errors }) =>
                <>
                    {fields.map((field, index) => (
                        <Form.Item {...field} label={index === 0 ? 'redirectUris' : ''} wrapperCol={index === 0 ? null : formItemLayoutWithOutLabel.wrapperCol}>
                            <Input.Group compact>
                                <Input style={{ width: '80%' }} required={true} />
                                <Button onClick={() => remove(field.name)} ><MinusCircleOutlined /></Button>
                            </Input.Group>
                        </Form.Item>
                    ))
                    }
                    <Button onClick={add}><PlusCircleOutlined /></Button>
                </>
            }
        </Form.List>
        {/* <Form.Item name={['endpoint', 'redirectUris']} label="redirectUri" rules={[{ required: true }]}>
            <Input />
        </Form.Item>
        <Form.Item name={['endpoint', 'postLogoutRedirectUris']} label="postLogoutRedirectUris" rules={[{ required: true }]}>
            <Input />
        </Form.Item> */}
        <Form.Item name={['endpoint', 'signIn']} label="signIn" rules={[{ required: true }]}>
            <Input />
        </Form.Item>
    </Form >
    );
};