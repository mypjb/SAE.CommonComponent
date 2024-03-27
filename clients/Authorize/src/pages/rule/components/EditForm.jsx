import { defaultFormBuild } from '@/utils/utils';
import { Form, Input } from 'antd';
import SymbolSelect from './SymbolSelect';

export default (props) => {
  const { model } = props;

  const [form] = Form.useForm();

  const [handleSave] = defaultFormBuild({
    ...props,
    form,
    dispatchType: 'rule/edit',
  });

  form.setFieldsValue(model);

  return (
    <Form form={form} onFinish={handleSave} size="middl">
      <Form.Item name="id" label="id" rules={[{ required: true }]} hidden>
        <Input />
      </Form.Item>
      <Form.Item name="name" label="name" rules={[{ required: true }]}>
        <Input />
      </Form.Item>
      <Form.Item
        name="description"
        label="description"
        rules={[{ required: true }]}
      >
        <Input />
      </Form.Item>
      <Form.Item name="left" label="left" rules={[{ required: true }]}>
        <Input />
      </Form.Item>
      <Form.Item name="symbol" label="symbol" rules={[{ required: true }]}>
        <SymbolSelect></SymbolSelect>
      </Form.Item>
      <Form.Item name="right" label="right" rules={[{ required: false }]}>
        <Input />
      </Form.Item>
    </Form>
  );
};
