import React from 'react';
import { Form, Input, Modal } from 'antd';
import { connect } from 'dva';

export default connect(({ project }) => ({ project }))(({ dispatch, project, visible }) => {

    const [form] = Form.useForm();

    const handleSave = (payload) => {
        dispatch({ type: 'project/edit', payload });
    }

    const handleOk = () => {
        form.submit();
    };

    const handleCancel = () => {
        dispatch({ type: 'project/setFormStaus', payload: 0 });
    };

    form.setFieldsValue(project.model);


    return (<Modal title="add" visible={visible}  onOk={handleOk} forceRender onCancel={handleCancel} closable={false}  >
        <Form form={form} size='middl' onFinish={handleSave} >
            <Form.Item name="name" label="name" rules={[{ required: true }]}>
                <Input />
            </Form.Item>
            <Form.Item name="solutionId" style={{ display: "none" }}>
                <Input />
            </Form.Item>
            <Form.Item name="id" style={{ display: "none" }}>
                <Input />
            </Form.Item>
        </Form>
    </Modal>);

})