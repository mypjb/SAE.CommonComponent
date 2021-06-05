import React from 'react';
import { Form, Input, Select } from 'antd';
import { validatorJson, handleFormat, defaultFormBuild } from '@/utils/utils';
import { useModel } from 'umi';

const { Option } = Select;
const { TextArea } = Input;

export default (props) => {

   
    // const model = useModel("template", model => {
    //     console.log(model);
    //     return model;
    // });
    console.log({ props });
    const { config } = props;

    const { solutionId } = config;

    const [form] = Form.useForm();

    const [handleSave] = defaultFormBuild({ ...props, form, dispatchType: "environment/add" });

    const handleSelectTemplate = (value, { data }) => {
        form.setFieldsValue({ content: data.format });
    }
    const handleFormatContent = handleFormat.bind(this, { form, fieldName: 'content' });

    const Options = [].map(data => <Option value={data.id} data={data}>{data.name}</Option>)

    return (
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
            <Form.Item name="solutionId" hidden>
                <Input value={solutionId} />
            </Form.Item>
        </Form>
    );
}