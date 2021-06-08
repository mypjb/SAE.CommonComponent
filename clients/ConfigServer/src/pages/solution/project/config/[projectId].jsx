import { PageHeaderWrapper } from '@ant-design/pro-layout';
import React, { useEffect } from 'react';
import { Row, Col, Input, Button, Modal, Select } from 'antd';
import { connect, useModel } from 'umi';
import RelevanceTable from './components/RelevanceTable';
import EditConfig from './components/EditConfig';
import PagingTable from '@/components/PagingTable';
import { defaultOperation, defaultDispatchType } from '@/utils/utils';

const { Search } = Input;


export default connect(({ projectConfig }) => (
  {
    projectConfig
  }))((props) => {

    const { dispatch, projectConfig, match } = props;

    const [modal, contextHolder] = Modal.useModal();

    const { environmentData } = useModel("environment", model => ({ environmentData: model.state }));

    const environmentOptions = environmentData.map(data => <Option value={data.id} data={data}>{data.name}</Option>);

    const defaultEnvId = environmentData.length ? environmentData[0].id : "";

    const dispatchType = defaultDispatchType("projectConfig");

    useEffect(() => {
      dispatch({
        type: dispatchType.search,
        payload: { ...match.params, environmentId: defaultEnvId }
      });
    }, []);


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

    const handleRelevance = () => {
      defaultOperation.add({ dispatch, element: RelevanceTable, ...match.params, modalProps: { width: "80%" } }, modal);
    };


    const handleEdit = (row) => {
      defaultOperation.edit({ dispatch, type: dispatchType.find, data: row.id, element: EditConfig });
    }

    const handleSearch = (keys) => {
      dispatch({
        type: dispatchType.search,
        payload: { ...keys, ...match.params },
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
      <PageHeaderWrapper>
        {contextHolder}
        <div>
          <Row>
            <Col span={18}>
              <Button type="primary" onClick={handleRelevance}>Relevance</Button>
              <Button type="primary" onClick={handleDelete}>Delete</Button>
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
          <PagingTable {...props}
            {...projectConfig}
            dispatchType={dispatchType.paging}
            type='projectConfig/paging'
            rowKey={columns[0].key}
            columns={columns}
            rowSelection={rowSelectOption} />
        </div>
      </PageHeaderWrapper>
    );
  });