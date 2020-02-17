import { PageHeaderWrapper } from '@ant-design/pro-layout';
import React from 'react';
import {  Row, Col, Input, Table } from 'antd';
import { connect } from 'dva';
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

export default connect(({solution})=>(
  {
    paging: solution
  }))(({ dispatch, paging }) => {

    const skipPage = (pageIndex, pageSize) => {
      dispatch({
        type:"solution/paging",
        payload:{
          pageIndex,
          pageSize
        }
      })
    }
  
    const handleSearch = (name) => {
      dispatch({
        type: 'solution/search',
        payload: {name},
      });
    }
  
    return (
      <PageHeaderWrapper className={styles.main}>
        <div>
        <Row>
            <Col span={18}></Col>
            <Col span={6}>
            <Search placeholder="input search text" onSearch={handleSearch}  className={styles.search} enterButton />
            </Col>
      </Row>
      <Table columns={columns} dataSource={paging.items} pagination={{
        current:paging.pageIndex,
        total:paging.totalCount,
        size:paging.pageSize,
        onChange:skipPage
      }}></Table>
        </div>
      </PageHeaderWrapper>
    );
  });

