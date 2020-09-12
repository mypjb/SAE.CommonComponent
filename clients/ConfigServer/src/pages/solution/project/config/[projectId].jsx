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


    const handleEdit = (e) => {
      dispatch({ type: 'projectConfig/query', payload: e.target.value });
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

    const handleSelect = (rowsKey, rowsData) => {
      ids = rowsData.map(s => (s.id));
    }

    const columns = [
      {
        title: 'id',
        dataIndex: 'id',
        key: 'id',
      },
      {
        title: 'alias',
        dataIndex: 'alias',
        key: 'alias',
      }, {
        title: 'action',
        render: (text, row) => (
          <span>
            <Button type='link' value={row.id} onClick={handleEdit} style={{ marginRight: 16 }}>Edit</Button>
            <Button type='link'>Config Manage</Button>
          </span>
        )
      }
    ];



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
          <Table columns={columns} dataSource={items} rowSelection={{ onChange: handleSelect }} pagination={pagination} />
          <Relevance visible={formStaus == 1} match={match}></Relevance>
          <EditConfig visible={formStaus == 2}></EditConfig>
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