import { PageHeaderWrapper } from '@ant-design/pro-layout';
import React from 'react';
import { Row, Col, Input, Table, Button, Modal } from 'antd';
import { connect,Link } from 'umi';
import styles from './index.less';
import AddForm from './components/AddForm';
import EditForm from './components/EditForm';

const { Search } = Input;

export default connect(({ solution }) => (
  {
    solution
  }))(({ dispatch, solution }) => {

    const { formStaus, paging, items } = solution;

    const handleRemove = (e) => {
      const id = e.target.value;
      Modal.confirm({
        title: 'Are you sure delete this task?',
        onOk: () => {
          dispatch({
            type: 'solution/remove',
            payload: { id },
          });
        }
      });
    }

    const handleAdd = () => {
      dispatch({ type: 'solution/setFormStaus', payload: 1 });
    }

    const handleEdit = (e) => {
      dispatch({ type: 'solution/query', payload: { id: e.target.value }, });
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
            <Button type='link' value={row.id} onClick={handleEdit} style={{ marginRight: 16 }}>Edit</Button>
            <Button type='link' value={row.id} onClick={handleRemove}>Delete</Button>
            <Link to={`/solution/project/${row.id}`} >
              <Button type='link'>Project Manage</Button>
            </Link>
            <Link to={`/solution/config/${row.id}`} >
              <Button type='link'>Config Manage</Button>
            </Link>
          </span>
        )
      }
    ];

    const pagination = {
      current: paging.pageIndex,
      total: paging.totalCount,
      size: paging.pageSize,
      onChange: handleSkipPage
    };

    return (
      <PageHeaderWrapper className={styles.main}>
        <div>
          <Row>
            <Col span={18}>
              <Button type="primary" onClick={handleAdd}>Add</Button>
            </Col>
            <Col span={6}>
              <Search placeholder="input search text" onSearch={handleSearch} className={styles.search} enterButton />
            </Col>
          </Row>
          <Table columns={columns} dataSource={items} pagination={pagination} />
          <AddForm visible={formStaus === 1} />
          <EditForm visible={formStaus === 2} />
        </div>
      </PageHeaderWrapper>
    );
  });