import { PageHeaderWrapper } from '@ant-design/pro-layout';
import React from 'react';
import { Row, Col, Input, Table, Button, Modal } from 'antd';
import { connect, Link, useModel } from 'umi';
import styles from './index.less';
import AddForm from './components/AddForm';
import EditForm from './components/EditForm';
import { defaultOperation, defaultDispatchType } from '@/utils/utils';
import PagingTable from '@/components/PagingTable';

const { Search } = Input;

export default connect(({ solution }) => (
  {
    solution
  }))((props) => {
    const { dispatch, solution } = props;
    const dispatchType = defaultDispatchType("solution");
    const handleDelete = (row) => {
      const id = row.id;
      Modal.confirm({
        title: 'Are you sure delete this task?',
        onOk: () => {
          dispatch({
            type: dispatchType.delete,
            payload: { id },
          });
        }
      });
    }

    const handleAdd = () => {
      defaultOperation.add({ dispatch, element: AddForm });
    }

    const handleEdit = (row) => {
      defaultOperation.edit({ dispatch, type: dispatchType.find, data: row.id, element: EditForm });
    }

    const handleSearch = (name) => {
      dispatch({
        type: dispatchType.add,
        payload: { name },
      });
    }

    const columns = [
      {
        title: 'serial number',
        dataIndex: 'id',
        key: 'id',
        render: (text, record, index) => {
          return index + 1;
        }
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
            <Button type='link' onClick={handleEdit.bind(null, row)} style={{ marginRight: 16 }}>Edit</Button>
            <Button type='link' onClick={handleDelete.bind(null, row)}>Delete</Button>
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

    return (
      <PageHeaderWrapper className={styles.main}>
        <div>
          <Row>
            <Col span={18}>
              <Button type="primary" onClick={handleAdd}>Add</Button>
              <Link to="/template">
                <Button type='primary'>Template</Button>
              </Link>
              <Link to="/environment">
                <Button type='primary'>Environment</Button>
              </Link>
            </Col>
            <Col span={6}>
              <Search placeholder="input search text" onSearch={handleSearch} className={styles.search} enterButton />
            </Col>
          </Row>
          <PagingTable {...props} {...solution} dispatchType={dispatchType.paging} columns={columns} />
        </div>
      </PageHeaderWrapper>
    );
  });