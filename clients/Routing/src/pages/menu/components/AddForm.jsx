import React from 'react';
import { Form, Input, Modal } from 'antd';
import { connect } from 'umi';

const { TextArea } = Input;

export default connect(({ menu }) => ({ menu }))(({ dispatch, menu, visible }) => {

    const [form] = Form.useForm();

    const handleSave = (payload) => {
        dispatch({ type: 'menu/add', payload });
    }


    if (!menu.model.parentName) {
        menu.model.parentName = "root";
    }

    form.setFieldsValue(menu.model);

    const handleOk = () => {
        form.submit();
    };

    const handleCancel = () => {
        dispatch({ type: 'menu/setFormStaus', payload: 0 });
    };

    return (<Modal title="add" visible={visible} onOk={handleOk} onCancel={handleCancel} closable={false}  >
        <Form form={form} onFinish={handleSave} size='middl'>
            <Form.Item name="name" label="name" rules={[{ required: true }]}>
                <Input />
            </Form.Item>
            <Form.Item name="parentName" label="parentName" rules={[{ required: true }]}>
                <Input disabled={true} />
            </Form.Item>
            <Form.Item name="parentId" label="parentId" style={{ display: "none" }}>
                <Input />
            </Form.Item>
            <Form.Item name="path" label="path" rules={[{ required: true }]}>
                <Input />
            </Form.Item>
        </Form>
    </Modal>
    );
})