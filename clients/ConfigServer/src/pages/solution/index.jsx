import { PageHeaderWrapper } from '@ant-design/pro-layout';
import React from 'react';
import { Row, Col, Input, Table, Button } from 'antd';
import { connect } from 'dva';
import { Link } from 'umi';
import styles from './index.less';

const { Search } = Input;

export default connect(({ solution }) => (
  {
    paging: solution
  }))(({ dispatch, paging }) => {

    const handleRemove = function () {
      dispatch({
        type: 'solution/remove',
        payload: { id: this },
      });
    }

    const handleSkipPage = (pageIndex, pageSize) => {
      dispatch({
        type: "solution/paging",
        payload: {
          pageIndex,
          pageSize
        }
      })
    }

    const handleSearch = (name) => {
      dispatch({
        type: 'solution/search',
        payload: { name },
      });
    }

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
      }, {
        title: 'createTime',
        dataIndex: 'createTime',
        key: 'createTime'
      }, {
        title: 'action',
        render: (text, row) => (
          <span>
            <Link to={(`/solution/edit/${row.id}`)} style={{ marginRight: 16 }}>Edit</Link>
            <Button type='link' onClick={handleRemove.bind(row.id)}>Delete</Button>
          </span>
        )
      }
    ];



    return (
      <PageHeaderWrapper className={styles.main}>
        <div>
          <Row>
            <Col span={18}>
              <Link to='/solution/add'>
                <Button type="primary">Add</Button>
              </Link>
            </Col>
            <Col span={6}>
              <Search placeholder="input search text" onSearch={handleSearch} className={styles.search} enterButton />
            </Col>
          </Row>
          <Table columns={columns} dataSource={paging.items} pagination={{
            current: paging.pageIndex,
            total: paging.totalCount,
            size: paging.pageSize,
            onChange: handleSkipPage
          }}></Table>
        </div>
      </PageHeaderWrapper>
    );
  });

