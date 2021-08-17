import { PageHeaderWrapper } from '@ant-design/pro-layout';
import React, { useEffect } from 'react';
import { Row, Col, Input, Table, Button, Modal, Select } from 'antd';
import { connect, Link, useModel } from 'umi';
import AddForm from './components/AddForm';
import EditForm from './components/EditForm';
import { defaultOperation, defaultDispatchType } from '@/utils/utils';
import PagingTable from '@/components/PagingTable';
import Preview from './components/Preview';

const { Search } = Input;
export default connect(({ app }) => (
  {
    app
  }))((props) => {
    const { dispatch, match, app } = props;

    const dispatchType = defaultDispatchType("app");

    const [modal, contextHolder] = Modal.useModal();

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
      defaultOperation.add({ dispatch, element: AddForm, clusterId: match.params.id });
    }

    const handleEdit = (row) => {
      defaultOperation.edit({ dispatch, type: dispatchType.find, data: row.id, element: EditForm });
    }

    const handlePublish = (row) => {
      modal.info({
        title: "Publish App Config",
        icon: (<></>),
        width: "80%",
        closable: true,
        cancelButtonProps: {
          hidden: true
        },
        okButtonProps: {
          hidden: true
        },
        content: (<Preview id={row.id} dispatch={dispatch}></Preview>)
      });
    }


    const handleSearch = (name) => {
      dispatch({
        type: this.dispatchType.search,
        payload: { name, ...match.params },
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
        dataIndex: 'name'
      },
      {
        title: 'createTime',
        dataIndex: 'createTime'
      }, {
        title: 'action',
        render: (text, row) => (
          <span>
            <Button type='link' onClick={handlePublish.bind(row, row)}>Publish</Button>
            <Button type='link' onClick={handleEdit.bind(row, row)} style={{ marginRight: 16 }}>Edit</Button>
            <Button type='link' onClick={handleDelete.bind(row, row)}>Delete</Button>
            <Link to={`/cluster/app/client/${row.id}`} >
              <Button type='link'>Client Manage</Button>
            </Link>
            <Link to={`/cluster/app/config/${row.id}`} >
              <Button type='link'>Config Manage</Button>
            </Link>
          </span>
        )
      }
    ];

    return (
      <PageHeaderWrapper>
        {contextHolder}
        <div>
          <Row>
            <Col span={18}>
              <Button type="primary" onClick={handleAdd}>Add</Button>
            </Col>
            <Col span={6}>
              <Search placeholder="input search text" onSearch={handleSearch} enterButton />
            </Col>
          </Row>
          <PagingTable columns={columns} {...props} {...app} dispatchType={dispatchType.paging} />
        </div>
      </PageHeaderWrapper>
    );
  });