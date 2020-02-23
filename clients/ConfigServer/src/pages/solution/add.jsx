import { PageHeaderWrapper } from '@ant-design/pro-layout';
import React from 'react';
import { Form, Input, Button } from 'antd';
import styles from './index.less';
import { connect } from 'dva';



export default connect()(({ dispatch }) => {
  const handleSave = (payload) => {
    dispatch({ type: 'solution/add', payload });
  }

  return (
    <PageHeaderWrapper className={styles.main}>
      <div>
        <Form layout="inline" onFinish={handleSave}>
          <Form.Item name="name" label="name" rules={[{ required: true }]}>
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