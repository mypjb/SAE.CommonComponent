import { PageHeaderWrapper } from '@ant-design/pro-layout';
import React from 'react';
import { Row, Col, Input, Table, Button, Modal } from 'antd';
import { connect } from 'umi';
import Relevance from './components/Relevance';
import EditConfig from './components/EditConfig';

const { Search } = Input;

class ProjectList extends React.Component {
  constructor(props) {
    super(props);
    props.dispatch({
      type: "projectConfig/search",
      payload: props.match.params
    });
  }


  render() {

    let ids = [];

    const { dispatch, projectConfig, match } = this.props;

    const { formStaus, paging, items } = projectConfig;

    const handleRemove = () => {
      Modal.confirm({
        title: 'Are you sure delete this task?',
        onOk: () => {
          dispatch({
            type: 'projectConfig/remove',
            payload: { ids },
          });
        }
      });
    }

    const handleRelevance = () => {
      dispatch({
        type: 'projectConfig/relevance',
        payload: match.params
      });

    };


    const handleEdit = (row) => {
      dispatch({
        type: 'projectConfig/find', payload: {
          id: row.id, callback: (data) => {
            Modal.confirm({
              title:"Edit",
              closable:false,
              content:(<EditConfig dispatch={dispatch} model={data}></EditConfig>)
            })
          }
        }
      });
    }

    const handleSkipPage = (pageIndex, pageSize) => {
      dispatch({
        type: "projectConfig/paging",
        payload: {
          pageIndex,
          pageSize
        }
      })
    }

    const handleSearch = (name) => {
      dispatch({
        type: 'projectConfig/search',
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
        title: 'alias',
        dataIndex: 'alias',
        key: 'alias',
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

    const pagination = {
      current: paging.pageIndex,
      total: paging.totalCount,
      size: paging.pageSize,
      onChange: handleSkipPage
    };


    return (
      <PageHeaderWrapper>
        <div>
          <Row>
            <Col span={18}>
              <Button type="primary" onClick={handleRelevance}>Relevance</Button>
              <Button type="primary" onClick={handleRemove}>Remove</Button>
            </Col>
            <Col span={6}>
              <Search placeholder="input search text" onSearch={handleSearch} enterButton />
            </Col>
          </Row>
          <Table rowKey={columns[0].key} columns={columns} dataSource={items} rowSelection={rowSelectOption} pagination={pagination} />
          <Relevance visible={formStaus == 1} match={match}></Relevance>
        </div>
      </PageHeaderWrapper>
    );
  }

}

export default connect(({ projectConfig, relevance }) => (
  {
    projectConfig,
    relevance
  }))(ProjectList);