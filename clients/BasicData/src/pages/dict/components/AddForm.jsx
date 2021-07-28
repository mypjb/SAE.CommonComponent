import React from 'react';
import { Form, Input, Select } from 'antd';
import { defaultFormBuild, Format } from '@/utils/utils';
import { dictType } from '@/utils/enum';

const { Option } = Select;

const rootKey = Object.keys(dictType)[0];

export default (props) => {

    let parent, model, parentElement;
    const dictTypes = [];

    if (props.parent) {
        parent = props.parent;
        dictTypes.push({ id: parent.type, name: Format.dictType(parent.type) });
        model = { parentId: parent.id, type: parent.type };
        parentElement = (<Form.Item name="parentId" label="parent" rules={[{ required: true }]}>
            <Select disabled={parent.id == 0}>
                <Option value={parent.id}>{parent.name}</Option>
            </Select>
        </Form.Item>);
    } else {
        parent = {
            id: 0,
            name: "root"
        };
        for (var key in dictType) {
            dictTypes.push({ id: dictType[key], name: key });
        }
        model = { parentId: parent.id, type: dictTypes[0].id };
        parentElement = (<React.Fragment></React.Fragment>);
    }

    const [form] = Form.useForm();

    const [handleSave] = defaultFormBuild({ ...props, form, dispatchType: "dict/add" });

    const dictOptions = dictTypes.map((val) => {
        return (<Option value={val.id}>{val.name}</Option>);
    });

    return (<Form form={form} onFinish={handleSave} size='middl' initialValues={model}>
        {parentElement}
        <Form.Item name="name" label="name" rules={[{ required: true }]}>
            <Input />
        </Form.Item>
        <Form.Item name="type" label="type" rules={[{ required: true }]}>
            <Select>
                {dictOptions}
            </Select>
        </Form.Item>
    </Form >
    );
};