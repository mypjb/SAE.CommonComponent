import React from 'react';
import { Form, Input, Modal, Select } from 'antd';
import { connect } from 'dva';
import { validatorJson, handleFormat } from '@/utils/utils';


const { Option } = Select;
const { TextArea } = Input;


export default connect(({ config }) => (
    {
        config
    }))(({ dispatch, visible, config }) => {

        const { params, templates } = config;

        const solutionId = params && params.solutionId ? params.solutionId : '';

        const [form] = Form.useForm();

        const handleSave = (payload) => {
            dispatch({ type: 'config/add', payload: { ...payload, solutionId } });
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

        const Options = templates.map(data => <Option value={data.id} data={data}>{data.name}</Option>)

        return (
            <Modal forceRender title="add" visible={visible} onOk={handleOk} onCancel={handleCancel} closable={false}>
                <Form form={form} size='middl' onFinish={handleSave}>
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
            </Modal>
        );
    })