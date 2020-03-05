import React from 'react';
import { Form, Input, Modal } from 'antd';
import { connect } from 'dva';

export default connect()(({ dispatch, visible }) => {
    const [form] = Form.useForm();
    const handleSave = (payload) => {
        dispatch({ type: 'solution/add', payload });
    }

    const handleOk = () => {
        form.submit();
    };

    const handleCancel = () => {
        dispatch({ type: 'solution/setFormStaus', payload: 0 });
    };

    return (
    <Modal forceRender title="add" visible={visible} onOk={handleOk} onCancel={handleCancel} closable={false}>
        <Form form={form} size='middl' onFinish={handleSave}>
            <Form.Item name="name" label="name" rules={[{ required: true }]}>
                <Input />
            </Form.Item>
        </Form>
    </Modal>
    );
})