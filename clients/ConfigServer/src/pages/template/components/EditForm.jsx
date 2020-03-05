import React from 'react';
import { Form, Input, Modal } from 'antd';
import { connect } from 'dva';
import { validatorJson } from '../../../utils/utils';

const { TextArea } = Input;

export default connect(({ template }) => ({ template }))(({ dispatch, template, visible }) => {

    const [form] = Form.useForm();

    const handleSave = (payload) => {
        dispatch({ type: 'template/edit', payload });
    }

    const handleOk = () => {
        form.submit();
    };

    const handleCancel = () => {
        dispatch({ type: 'template/setFormStaus', payload: 0 });
    };

    form.setFieldsValue(template.model);

    const handleFormat = (e) => {
        const value = e.target.value;
        if (value) {
            try {
                const json = eval('(' + value + ')');
                form.setFieldsValue({ format: JSON.stringify(json, null, 4) });
            } catch{ }
        }
    };

    return (<Modal title="add" visible={visible} onOk={handleOk} onCancel={handleCancel} closable={false}  >
        <Form form={form} size='middl' onFinish={handleSave} >
            <Form.Item name="id" style={{ display: "none" }}>
                <Input />
            </Form.Item>
            <Form.Item name="name" label="name" rules={[{ required: true }]}>
                <Input />
            </Form.Item>
            <Form.Item name="format" label="format" rules={[{ validator: validatorJson, required: true }]}>
                <TextArea autoSize={{ minRows: 16 }} onDoubleClick={handleFormat} />
            </Form.Item>
        </Form>
    </Modal>);

})