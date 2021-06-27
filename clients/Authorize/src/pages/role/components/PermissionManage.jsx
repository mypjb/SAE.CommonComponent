import { PageHeaderWrapper } from '@ant-design/pro-layout';
import React, { useState, useEffect } from 'react';
import { Row, Col, Input, Button } from 'antd';
import { defaultOperation, defaultDispatchType, defaultState } from '@/utils/utils';
import PagingTable from '@/components/PagingTable';

const { Search } = Input;

export default (props) => {

  const { dispatch, role } = props;

  const dispatchType = defaultDispatchType("permission");

  const [state, setState] = useState({ ...defaultState, params: { id: role.id, referenced: false } });

  const { paging } = state;

  const handleSkipPage = (pageIndex, pageSize) => {
    dispatch({
      type: dispatchType.paging,
      payload: {
        pageIndex,
        pageSize,
        ...state.params,
        callback: (data) => {
          setState({ ...data, params: state.params });
        }
      }
    });
  };

  useEffect(() => {
    handleSkipPage(paging.pageIndex, paging.pageSize);
  }, []);

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
    , {
      title: 'createTime',
      dataIndex: 'createTime'
    }
  ];

  return (
    <PageHeaderWrapper >
      <div>
        <Row>
          <Col span={18}>
          </Col>
          <Col span={6}>
          </Col>
        </Row>
        <PagingTable {...props}  {...state} handleSkipPage={handleSkipPage} columns={columns} />
      </div>
    </PageHeaderWrapper>
  );
};

