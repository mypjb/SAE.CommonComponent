import React from 'react';
import { Form, Input, Modal } from 'antd';
import { connect } from 'umi';
import { validatorJson, handleFormat } from '@/utils/utils';

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

    const handleFormatFormat = handleFormat.bind(this, { form, fieldName: 'format' });

    return (<Modal title="add" visible={visible} onOk={handleOk} onCancel={handleCancel} closable={false}  >
        <Form form={form} onFinish={handleSave} size='middl'>
            <Form.Item name="name" label="name" rules={[{ required: true }]}>
                <Input />
            </Form.Item>
            <Form.Item name="format" label="format" rules={[{ validator: validatorJson, required: true }]}>
                <TextArea autoSize={{ minRows: 16 }} onDoubleClick={handleFormatFormat} />
            </Form.Item>
        </Form>
    </Modal>
    );
})