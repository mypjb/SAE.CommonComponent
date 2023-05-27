import React, { useEffect } from 'react';
import { Row, Col, Input, Button, Modal, Select } from 'antd';
import { connect, useModel, useParams } from 'umi';
import ReferenceTable from './components/ReferenceTable';
import EditConfig from './components/EditConfig';
import PagingTable from '@/components/PagingTable';
import { defaultOperation, defaultDispatchType } from '@/utils/utils';
import { useState } from 'react';

const { Search } = Input;


export default connect(({ appConfig }) => (
  {
    appConfig
  }))((props) => {

    const match = useParams();

    const { dispatch, appConfig } = props;

    const appId = match.id;

    const [modal, contextHolder] = Modal.useModal();

    const { environmentData } = useModel("environment", model => ({ environmentData: model.state }));

    const environmentOptions = environmentData.map(data => <Option value={data.id} data={data}>{data.name}</Option>);

    const [environmentId, setEnvironment] = useState(environmentData.length ? environmentData[0].id : "");

    const dispatchType = defaultDispatchType("appConfig");

    useEffect(() => {
      if (environmentId) {
        handleSearch({ environmentId, alias: "" });
      }
    }, [appId]);


    let ids = [];

    const handleDelete = () => {
      Modal.confirm({
        title: 'Are you sure delete this task?',
        onOk: () => {
          dispatch({
            type: dispatchType.delete,
            payload: { ids },
          });
        }
      });
    }

    const handleReference = () => {
      defaultOperation.add({
        dispatch,
        element: ReferenceTable,
        appId,
        modalProps: { width: "80%" }
      }, modal);
    };


    const handleEdit = (row) => {
      defaultOperation.edit({
        dispatch,
        type: dispatchType.find,
        data: row.id,
        element: EditConfig
      });
    }

    const handleSearch = (data) => {
      if (data.environmentId) {
        setEnvironment(data.environmentId);
      }
      console.log(data);
      dispatch({
        type: dispatchType.search,
        payload: { appId, ...data },
      });
    }

    const columns = [
      {
        title: 'serial number',
        dataIndex: 'id',
        render: (text, record, index) => {
          return index + 1;
        }
      },
      {
        title: 'alias',
        dataIndex: 'alias',
      }, {
        title: 'action',
        render: (text, row) => (
          <span>
            <Button type='link' onClick={handleEdit.bind(row, row)} style={{ marginRight: 16 }}>Edit</Button>
          </span>
        )
      }
    ];

    const rowSelectOption = {
      preserveSelectedRowKeys: false,
      onChange: (rowsKey, rowsData) => {
        ids = rowsData.map(s => (s.id));
      }
    }


    return (
      <div>
        {contextHolder}
        <Row>
          <Col span={18}>
            <Button type="primary" onClick={handleReference}>Reference</Button>
            <Button type="primary" onClick={handleDelete}>Delete</Button>
          </Col>
          <Col span={2}>
            <Select style={{ width: '100%' }} value={environmentId} onChange={(environmentId) => handleSearch({ environmentId })}>
              {environmentOptions}
            </Select>
          </Col>
          <Col span={4}>
            <Search placeholder="input search text" onSearch={(alias) => handleSearch({ alias })} enterButton />
          </Col>
        </Row>
        <PagingTable {...props}
          {...appConfig}
          dispatchType={dispatchType.paging}
          type='appConfig/paging'
          rowKey='id'
          columns={columns}
          rowSelection={rowSelectOption} />
      </div>
    );
  });
