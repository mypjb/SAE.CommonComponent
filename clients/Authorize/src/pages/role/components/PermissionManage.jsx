import { PageHeaderWrapper } from '@ant-design/pro-layout';
import React, { useState, useEffect } from 'react';
import { Row, Col, Select } from 'antd';
import { defaultOperation, defaultDispatchType, defaultState, Format } from '@/utils/utils';
import PagingTable from '@/components/PagingTable';

const { Option } = Select;

export default (props) => {

  const { dispatch, role } = props;

  const dispatchType = defaultDispatchType("rolePermission");

  const [state, setState] = useState({
    ...defaultState,
    params: {
      id: role.id, referenced: true
    },
    permissionIds: []
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
          setState({ ...state, ...data, params: state.params, permissionIds: [] });
        }
      }
    });
  };

  const handleChange = (referenced) => {
    setState({ ...state, params: { ...state.params, referenced } });
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
    }
    , {
      title: 'status',
      dataIndex: 'status',
      render: (status, row) => {
        return Format.status(status);
      }
    },
    {
      title: 'descriptor',
      dataIndex: 'descriptor'
    },
    {
      title: 'createTime',
      dataIndex: 'createTime',
      render: Format.date
    }
  ];

  const rowSelectionOption = {
    onChange: (selectedRowKeys, selectedRows) => {
      console.log(selectedRowKeys);
      const permissionIds = selectedRows.map(s => (s.id));
      setState({ ...state, permissionIds });
    },
    selectedRowKeys: state.permissionIds
  }

  props.okCallback((close) => {
    dispatch({
      type: state.params.referenced ? dispatchType.delete : dispatchType.add,
      payload: {
        permissionIds: state.permissionIds,
        id: role.id,
        callback: function () {
          handleSkipPage();
        }
      }
    });
    return false;
  });

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
          rowKey="id"
          rowSelection={rowSelectionOption} />
      </div>
    </PageHeaderWrapper>
  );
};

