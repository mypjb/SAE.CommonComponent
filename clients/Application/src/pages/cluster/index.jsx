import { PageHeaderWrapper } from '@ant-design/pro-layout';
import React, { useEffect } from 'react';
import { Row, Col, Input, Table, Button, Modal } from 'antd';
import { connect, Link, useModel } from 'umi';
import AddForm from './components/AddForm';
import EditForm from './components/EditForm';
import { defaultOperation, defaultDispatchType, Format } from '@/utils/utils';
import PagingTable from '@/components/PagingTable';

const { Search } = Input;

export default connect(({ cluster }) => (
  {
    cluster
  }))((props) => {
    const { dispatch, cluster } = props;
    const dispatchType = defaultDispatchType("cluster");
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

    const handleStatus = (row) => {
      dispatch({
        type: "cluster/status",
        payload: {
          id: row.id,
          status: Math.abs(row.status - 1)
        }
      })
    };

    const handleSearch = (name) => {
      dispatch({
        type: dispatchType.search,
        payload: { name },
      });
    }

    useEffect(() => {
      handleSearch();
    }, []);

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
        title: 'status',
        dataIndex: 'status',
        render: (status, row) => {
          return Format.status(status, { onClick: handleStatus.bind(row, row) });
        }
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
            <Link to={`/cluster/app/${row.id}`} >
              <Button type='link'>App Manage</Button>
            </Link>
            <Link to={`/cluster/config/${row.id}`} >
              <Button type='link'>Config Manage</Button>
            </Link>
          </span>
        )
      }
    ];

    return (
      <div>
        <Row>
          <Col span={18}>
            <Button type="primary" onClick={handleAdd}>Add</Button>
          </Col>
          <Col span={6}>
            <Search placeholder="input search text" onSearch={handleSearch} enterButton />
          </Col>
        </Row>
        <PagingTable {...props} {...cluster} dispatchType={dispatchType.paging} columns={columns} />
      </div>
    );
  });