import { PageHeaderWrapper } from '@ant-design/pro-layout';
import React, { useState, useEffect } from 'react';
import { Row, Col, Select, Button } from 'antd';
import { defaultOperation, defaultDispatchType, defaultState, Format } from '@/utils/utils';
import PagingTable from '@/components/PagingTable';

const { Option } = Select;

export default (props) => {

  const { dispatch, app } = props;

  const dispatchType = defaultDispatchType("appProject");

  const [state, setState] = useState({
    ...defaultState,
    params: {
      id: app.id,
      referenced: true
    }
  });

  const { paging } = state;


  const handleSkipPage = (pageIndex, pageSize) => {
    dispatch({
      type: dispatchType.paging,
      payload: {
        pageIndex,
        pageSize,
        ...state.params,
        callback: (data) => {
          setState({ ...state, ...data, params: state.params });
        }
      }
    });
  };

  const handleChange = (referenced) => {
    setState({ ...state, params: { ...state.params, referenced } });
  }

  const handleProject = (row) => {
    dispatch({
      type: dispatchType.add,
      payload: {
        id: app.id,
        projectId: row.id,
        callback: function () {
          handleSkipPage();
        }
      }
    });
  }

  useEffect(() => {
    handleSkipPage(paging.pageIndex, paging.pageSize);
  }, [state.params.referenced]);

  const columns = [
    {
      title: 'serial number',
      dataIndex: 'id',
      render: (text, record, index) => {
        return index + 1;
      }
    },
    {
      title: 'name',
      dataIndex: 'name'
    },
    {
      title: 'solutionName',
      dataIndex: 'solutionName'
    },
    {
      title: 'createTime',
      dataIndex: 'createTime',
      render: Format.date
    },
    {
      title: 'action',
      render: (text, row) => (
        state.params.referenced ? (<></>) : (<Button type='link' onClick={handleProject.bind(row, row)}>Reference Project</Button>)
      )
    }
  ];

  return (
    <PageHeaderWrapper >
      <div>
        <Row>
          <Col span={18}>
          </Col>
          <Col span={6}>
            <Select onChange={handleChange} defaultValue={state.params.referenced}>
              <Option value={true}>Reference</Option>
              <Option value={false}>No Reference</Option>
            </Select>
          </Col>
        </Row>
        <PagingTable {...props}
          {...state}
          handleSkipPage={handleSkipPage}
          columns={columns}
          rowKey="id" />
      </div>
    </PageHeaderWrapper>
  );
};

