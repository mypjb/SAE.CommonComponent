import React from 'react';
import { Form, Input, Modal } from 'antd';
import { connect } from 'dva';

export default connect(({ solution }) => ({ solution }))(({ dispatch, solution, visible }) => {

    const [form] = Form.useForm();

    const handleSave = (payload) => {
        dispatch({ type: 'solution/edit', payload });
    }

    const handleOk = () => {
        form.submit();
    };

    const handleCancel = () => {
        dispatch({ type: 'solution/setFormStaus', payload: 0 });
    };

    form.setFieldsValue(solution.model);


    return (<Modal title="add" visible={visible} onOk={handleOk} onCancel={handleCancel} closable={false}  >
        <Form form={form} layout="inline" onFinish={handleSave} >
            <Form.Item name="name" label="name" rules={[{ required: true }]}>
                <Input />
            </Form.Item>
            <Form.Item name="id" style={{ display: "none" }}>
                <Input />
            </Form.Item>
        </Form>
    </Modal>);

})