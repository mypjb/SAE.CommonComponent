import React from 'react';
import { Form, Input, Select } from 'antd';
import { useModel } from 'umi';
import {  defaultFormBuild } from '@/utils/utils';
import { validatorJson, handleFormat } from '@/utils/utils.extension';

const { Option } = Select;
const { TextArea } = Input;

export default (props) => {

    const { model } = props;

    const { templateData } = useModel("template", model => ({ templateData: model.state }));

    const { environmentData } = useModel("environment", model => ({ environmentData: model.state }));

    const [form] = Form.useForm();

    const [handleSave] = defaultFormBuild({ ...props, form, dispatchType: "config/edit" });

    const handleSelectTemplate = (value, { data }) => {
        form.setFieldsValue({ content: data.format });
    }

    const handleFormatContent = handleFormat.bind(this, { form, fieldName: 'content' });

    const templateOptions = templateData.map(data => <Option value={data.id} data={data}>{data.name}</Option>);

    const environmentOptions = environmentData.map(data => <Option value={data.id} data={data}>{data.name}</Option>);

    form.setFieldsValue(model);

    return (<Form form={form} size='middle' onFinish={handleSave} >
        <Form.Item name="id" hidden>
            <Input />
        </Form.Item>
        <Form.Item name="clusterId" hidden>
            <Input />
        </Form.Item>
        <Form.Item name="name" label="name" rules={[{ required: true }]}>
            <Input />
        </Form.Item>
        <Form.Item name="environmentId" label="environment" >
            <Select style={{ width: 200 }} disabled>
                {environmentOptions}
            </Select>
        </Form.Item>
        <Form.Item name="templateId" label="template" >
            <Select onChange={handleSelectTemplate} showSearch placeholder="Select a tempalte" optionFilterProp="children" style={{ width: 200 }}>
                {templateOptions}
            </Select>
        </Form.Item>
        <Form.Item name="content" label="content" rules={[{ validator: validatorJson, required: true }]}>
            <TextArea autoSize={{ minRows: 16 }} onDoubleClick={handleFormatContent} />
        </Form.Item>
    </Form>);

}