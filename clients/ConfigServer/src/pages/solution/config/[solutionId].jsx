import { PageHeaderWrapper } from '@ant-design/pro-layout';
import React, { useEffect } from 'react';
import { Row, Col, Input, Table, Button, Modal, Select } from 'antd';
import { connect, useModel } from 'umi';
import AddForm from './components/AddForm';
import EditForm from './components/EditForm';
import PagingTable from '@/components/PagingTable';
import { defaultOperation, defaultDispatchType, defaultHandler } from '@/utils/utils';

const { Search } = Input;
const { Option } = Select;

export default connect(({ config }) => (
  {
    config
  }))((props) => {


    const { dispatch, config, match } = props;


    const { environmentData } = useModel("environment", model => ({ environmentData: model.state }));
    const defaultEnvId = environmentData.length ? environmentData[0].id : "";
    const environmentOptions = environmentData.map(data => <Option value={data.id} data={data}>{data.name}</Option>);

    const dispatchType = defaultDispatchType("config");

    const [modal, contextHolder] = Modal.useModal();

    useEffect(() => {
      dispatch({
        type: dispatchType.search,
        payload: {
          environmentId: defaultEnvId,
          ...match.params
        }
      });
    }, []);

    const handleDelete = defaultHandler.delete({ dispatch, dispatchType: dispatchType.delete });

    const handleAdd = () => {
      defaultOperation.add({ dispatch, element: AddForm, ...match.params }, modal);
    }

    const handleEdit = (row) => {
      defaultOperation.edit({ dispatch, type: dispatchType.find, data: row.id, element: EditForm }, modal);
    }

    const handleSearch = defaultHandler.search({ dispatch, dispatchType: dispatchType.search });

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
        title: 'env',
        dataIndex: 'environmentId',
        render: (environmentId, row) => {
          const index = environmentData.findIndex((env, index) => {
            return env.id == environmentId;
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
            <Button type='link' onClick={handleEdit.bind(null, row)} style={{ marginRight: 16 }}>Edit</Button>
            <Button type='link' onClick={handleDelete.bind(null, row)}>Delete</Button>
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
            <Col span={2}>
              <Select style={{ width: '100%' }} defaultValue={defaultEnvId} onChange={(environmentId) => handleSearch({ environmentId })}>
                {environmentOptions}
              </Select>
            </Col>
            <Col span={4}>
              <Search placeholder="input search text" onSearch={(name) => handleSearch({ name })} enterButton />
            </Col>
          </Row>
          <PagingTable {...props} {...config} dispatchType={dispatchType.paging} columns={columns} />
        </div>
      </PageHeaderWrapper>
    );
  })