import React from 'react';
import { Form, Input, Modal, Select } from 'antd';
import { connect } from 'dva';
import { validatorJson, handleFormat } from '@/utils/utils';

const { Option } = Select;
const { TextArea } = Input;

export default connect(({ config }) => ({ config }))(({ dispatch, config, visible }) => {

    const [form] = Form.useForm();

    const handleSave = (payload) => {        
        dispatch({ type: 'config/edit', payload });
    }

    const handleOk = () => {
        form.submit();
    };

    const handleCancel = () => {
        dispatch({ type: 'config/setFormStaus', payload: 0 });
    };

    const handleSelectTemplate = (value, { data }) => {
        form.setFieldsValue({ content: data.format });
    }

    const handleFormatContent = handleFormat.bind(this, { form, fieldName: 'content' });

    const Options = config.templates.map(data => <Option value={data.id} data={data}>{data.name}</Option>)

    form.setFieldsValue(config.model);

    return (<Modal title="add" visible={visible} onOk={handleOk} forceRender onCancel={handleCancel} closable={false}  >
        <Form form={form} size='middl' onFinish={handleSave} >
            <Form.Item name="id" style={{ display: "none" }}>
                <Input />
            </Form.Item>
            <Form.Item name="solutionId" style={{ display: "none" }}>
                <Input />
            </Form.Item>
            <Form.Item name="name" label="name" rules={[{ required: true }]}>
                <Input />
            </Form.Item>
            <Form.Item name="templateId" label="template" >
                <Select onChange={handleSelectTemplate} showSearch placeholder="Select a tempalte" optionFilterProp="children" style={{ width: 200 }}>
                    {Options}
                </Select>
            </Form.Item>
            <Form.Item name="content" label="content" rules={[{ validator: validatorJson, required: true }]}>
                <TextArea autoSize={{ minRows: 16 }} onDoubleClick={handleFormatContent} />
            </Form.Item>
        </Form>
    </Modal>);

})