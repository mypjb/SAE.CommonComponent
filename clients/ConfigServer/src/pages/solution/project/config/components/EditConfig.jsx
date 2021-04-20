import React from 'react';
import { Form, Input, Modal } from 'antd';
import { connect } from 'umi';

// export default connect(({ projectConfig }) => ({ projectConfig }))(({ dispatch, projectConfig, visible }) => {
export default (props) => {
    console.log(props);
    debugger
    const { dispatch, model } = props;
    const [form] = Form.useForm();

    const handleSave = (payload) => {
        dispatch({ type: 'projectConfig/edit', payload });
    }

    const handleOk = () => {
        form.submit();
    };

    const handleCancel = () => {
        dispatch({ type: 'project/setFormStaus', payload: 0 });
    };

    form.setFieldsValue(model);

    return (<Form form={form} size='middl' onFinish={handleSave} >
        <Form.Item name="alias" label="alias" rules={[{ required: true }]}>
            <Input />
        </Form.Item>
        <Form.Item name="id" style={{ display: "none" }}>
            <Input />
        </Form.Item>
    </Form>);
    // return (<Modal title="add" visible={visible}  onOk={handleOk} forceRender onCancel={handleCancel} closable={false}  >
    //     <Form form={form} size='middl' onFinish={handleSave} >
    //         <Form.Item name="alias" label="alias" rules={[{ required: true }]}>
    //             <Input />
    //         </Form.Item>
    //         <Form.Item name="id" style={{ display: "none" }}>
    //             <Input />
    //         </Form.Item>
    //     </Form>
    // </Modal>);

};