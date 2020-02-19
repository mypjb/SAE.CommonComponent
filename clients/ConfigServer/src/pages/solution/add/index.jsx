import { PageHeaderWrapper } from '@ant-design/pro-layout';
import React from 'react';
import { Form, Input, Button } from 'antd';
import styles from './index.less';

const BasicForm = Form.create({})((props) => {
  const handleSubmit = (e)=>{
    e.preventDefault();
    props.form.validateFields((err, values) => {
      if (!err) {
        console.log('Received values of form: ', values);
      }
    });
  };
  
  const { getFieldDecorator}= props.form;

  return (<Form layout="inline" onSubmit={handleSubmit}>
    <Form.Item label="name">
      {getFieldDecorator('name',{
        rules:[
          {
            required: true,
            message: 'Please input your name',
          }
        ]
      })(<Input></Input>)}
    </Form.Item>
    <Form.Item>
    <Button type="primary" htmlType="submit" >
      Register
    </Button>
  </Form.Item>
  </Form>);
});


export default function () {
  return (
    <PageHeaderWrapper  className={styles.main}>
      <div>
        <BasicForm></BasicForm>
      </div>
    </PageHeaderWrapper>
  );
};
