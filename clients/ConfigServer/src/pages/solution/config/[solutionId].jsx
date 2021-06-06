import { PageHeaderWrapper } from '@ant-design/pro-layout';
import React from 'react';
import { Row, Col, Input, Table, Button, Modal } from 'antd';
import { connect, useModel } from 'umi';
import AddForm from './components/AddForm';
import EditForm from './components/EditForm';
import PagingTable from '@/components/PagingTable';
import { defaultOperation, defaultDispatchType } from '@/utils/utils';

const { Search } = Input;

export default connect(({ config }) => (
  {
    config
  }))((props) => {
    const { dispatch, config, match } = props;

    const { environmentData } = useModel("environment", model => ({ environmentData: model.state }));

    const dispatchType = defaultDispatchType("config");

    const [modal, contextHolder] = Modal.useModal();

    const handleDelete = (row) => {
      Modal.confirm({
        title: 'Are you sure delete this task?',
        onOk: () => {
          dispatch({
            type: dispatchType.delete,
            payload: { id: row.id },
          });
        }
      });
    }

    const handleAdd = () => {
      defaultOperation.add({ dispatch, element: AddForm, config, ...match.params }, modal);
    }

    const handleEdit = (row) => {
      defaultOperation.edit({ dispatch, type: dispatchType.find, data: row.id, element: EditForm });
    }

    const handleSearch = (name) => {
      dispatch({
        type: dispatchType.search,
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
        title: 'environmentId',
        dataIndex: 'environmentId',
        key: 'environment',
        render: (environmentId, row) => {
          const index = environmentData.findIndex((value, index) => {
            return value == environmentId;
          }); 
          return index > -1 ? environmentData[index].name : "--";
        }
      },
      {
        title: 'content',
        dataIndex: 'content',
        ellipsis: true
      },
      {
        title: 'createTime',
        dataIndex: 'createTime'
      },
      {
        title: 'action',
        render: (text, row) => (
          <span>
            <Button type='link' value={row.id} onClick={handleEdit} style={{ marginRight: 16 }}>Edit</Button>
            <Button type='link' value={row.id} onClick={handleDelete}>Delete</Button>
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
          <PagingTable {...props} {...config} dispatchType={dispatchType.paging} columns={columns} />
        </div>
      </PageHeaderWrapper>
    );
  })