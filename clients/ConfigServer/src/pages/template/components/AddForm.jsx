import React from 'react';
import { Form, Input, Modal } from 'antd';
import { connect } from 'dva';
import { validatorJson } from '@/utils/utils';

const { TextArea } = Input;

export default connect()(({ dispatch, visible }) => {

    const [form] = Form.useForm();

    const handleSave = (payload) => {
        dispatch({ type: 'template/add', payload });
    }

    const handleOk = () => {
        form.submit();
    };

    const handleCancel = () => {
        dispatch({ type: 'template/setFormStaus', payload: 0 });
    };

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
        <Form form={form} onFinish={handleSave} size='middl'>
            <Form.Item name="name" label="name" rules={[{ required: true }]}>
                <Input />
            </Form.Item>
            <Form.Item name="format" label="format" rules={[{ validator: validatorJson, required: true }]}>
                <TextArea autoSize={{ minRows: 16 }} onDoubleClick={handleFormat} />
            </Form.Item>
        </Form>
    </Modal>
    );
})