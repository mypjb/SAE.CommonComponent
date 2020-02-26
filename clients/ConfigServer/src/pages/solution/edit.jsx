import { PageHeaderWrapper } from '@ant-design/pro-layout';
import React from 'react';
import { Form, Input, Button } from 'antd';
import { connect } from 'dva';
import styles from './index.less';

export default connect(({ solution }) => ({ solution }))(({ dispatch, solution, match }) => {

  const [form] = Form.useForm();

  const handleSave = (payload) => {
    dispatch({ type: 'solution/edit', payload });
  }

  form.setFieldsValue(solution.model);

  return (
    <PageHeaderWrapper className={styles.main}>
      <div>
        <Form form={form} layout="inline" onFinish={handleSave} >
          <Form.Item name="name" label="name" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item name="id" style={{ display: "none" }}>
            <Input />
          </Form.Item>
          <Form.Item>
            <Button type="primary" htmlType="submit" >
              Registerty
            </Button>
          </Form.Item>
        </Form>
      </div>
    </PageHeaderWrapper>
  );

})