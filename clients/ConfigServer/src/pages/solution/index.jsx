import { PageHeaderWrapper } from '@ant-design/pro-layout';
import React, { useState, useEffect } from 'react';
import { Spin, Row, Col, Input, Table } from 'antd';
import { queryPaging } from '@/services/solution';
import styles from './index.less';

const {Search}=Input;



const columns = [
  {
    title: 'id',
    dataIndex: 'id',
    key: 'id',
  },
  {
    title: 'name',
    dataIndex: 'name',
    key: 'name',
  },{
    title: 'createTime',
    dataIndex: 'createTime',
    key: 'createTime'
  }
];

const QueryTable = () => {

  const [data, setData] = useState({items:[]});
 
  const skip = params=>{
    queryPaging(params).then(value=>{
      setData(value);
    });
  };

  const handleSearch=name=>{
    skip({ name });
  };
  
  const skipPage=(pageIndex,pageSize)=>{
    skip({pageIndex,pageSize});
  };
  return <div>
    <Row>
          <Col span={18}></Col>
          <Col span={6}>
          <Search placeholder="input search text" onSearch={handleSearch}  className={styles.search} enterButton />
          </Col>
    </Row>
    <Table columns={columns} dataSource={data.items} pagination={{
      current:data.pageIndex,
      total:data.totalCount,
      size:data.pageSize,
      onChange:skipPage
    }}></Table>
  </div>
}

export default () => {
  const [loading, setLoading] = useState(true);
  useEffect(() => {
    setTimeout(() => {
      setLoading(false);
    }, 3000);
  }, []);
  return (
    <PageHeaderWrapper className={styles.main}>
      <div>
        <Spin spinning={loading} size="large"></Spin>
        <QueryTable></QueryTable>
      </div>
    </PageHeaderWrapper>
  );
};
