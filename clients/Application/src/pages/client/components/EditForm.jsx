import React from 'react';
import { Button, Form, Input, Select } from 'antd';
import { defaultFormBuild, guid } from '@/utils/utils';
import { useModel } from 'umi';
import { MinusCircleOutlined, PlusCircleOutlined } from '@ant-design/icons';
const { Option } = Select;


export default (props) => {

    const { model } = props;

    const { scope } = useModel('scope', model => ({ scope: model.state, load: model.load }));

    const [form] = Form.useForm();

    const [handleSave] = defaultFormBuild({ ...props, form, dispatchType: "client/edit" });

    const scopeOptions = scope.map((val) => {
        return (<Option value={val.id}>{val.name}</Option>);
    });

    form.setFieldsValue(model);

    return (<Form form={form} onFinish={handleSave} size='middl' >
        <Form.Item name="id" label="id" rules={[{ required: true }]} hidden>
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
        <Form.List name={['endpoint', 'redirectUris']}>
            {(fields, { add, remove }) => (
                fields.map((field, index) => (
                    <Form.Item
                        {...field}
                        label={index === 0 ? 'redirectUri' : (<span style={{ color: "transparent" }}>* redirectUri:</span>)}
                        rules={[{ required: true }]}
                        required={index === 0}
                        colon={index === 0}
                        validateTrigger={['onChange', 'onBlur']}>
                        <Input addonAfter={index === 0 ? <PlusCircleOutlined onClick={() => add()} /> : <MinusCircleOutlined onClick={() => remove(field.name)} />} />
                    </Form.Item>
                ))
            )}
        </Form.List>
        <Form.List name={['endpoint', 'postLogoutRedirectUris']}>
            {(fields, { add, remove }) => (
                fields.map((field, index) => (
                    <Form.Item
                        {...field}
                        label={index === 0 ? 'LogoutRedirect' : (<span style={{ color: "transparent" }}>* LogoutRedirect:</span>)}
                        rules={[{ required: true }]}
                        required={index === 0}
                        colon={index === 0}
                        validateTrigger={['onChange', 'onBlur']}>
                        <Input addonAfter={index === 0 ? <PlusCircleOutlined onClick={() => add()} /> : <MinusCircleOutlined onClick={() => remove(field.name)} />} />
                    </Form.Item>
                ))
            )}
        </Form.List>
        <Form.Item name={['endpoint', 'signIn']} label="signIn" rules={[{ required: true }]}>
            <Input />
        </Form.Item>
    </Form >
    );
};