import { PageHeaderWrapper } from '@ant-design/pro-layout';
import React, { useEffect } from 'react';
import { Row, Col, Input, Button, Modal } from 'antd';
import { connect } from 'umi';
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

    dispatchType = defaultDispatchType("projectConfig");

    useEffect(() => {
      dispatch({
        type: dispatchType.search,
        payload: match.params
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
      defaultOperation.add({ dispatch, element: RelevanceTable, ...match.params });
    };


    const handleEdit = (row) => {
      defaultOperation.edit({ dispatch, type: dispatchType.find, data: row.id, element: EditConfig });
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
            <Button type='link'>Config Manage</Button>
          </span>
        )
      }
    ];

    const rowSelectOption = {
      onChange: (rowsKey, rowsData) => {
        ids = rowsData.map(s => (s.id));
      }
    }


    return (
      <PageHeaderWrapper>
        <div>
          <Row>
            <Col span={18}>
              <Button type="primary" onClick={handleRelevance}>Relevance</Button>
              <Button type="primary" onClick={handleDelete}>Delete</Button>
            </Col>
            <Col span={6}>
              <Search placeholder="input search text" onSearch={handleSearch} enterButton />
            </Col>
          </Row>
          <PagingTable {...this.props}
            {...projectConfig}
            dispatchType={this.dispatchType.paging}
            type='projectConfig/paging'
            rowKey={columns[0].key}
            columns={columns}
            rowSelection={rowSelectOption} />
        </div>
      </PageHeaderWrapper>
    );
  });