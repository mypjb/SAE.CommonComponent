import { PageHeaderWrapper } from '@ant-design/pro-layout';
import React from 'react';
import { Row, Col, Input, Table, Button, Modal } from 'antd';
import { connect } from 'dva';
import Relevance from './components/Relevance';

const { Search } = Input;

class ProjectList extends React.Component {
  constructor(props) {
    super(props);
    props.dispatch({
      type: "config/paging",
      payload: props.match.params
    });
  }


  render() {
    const { dispatch, paging, match, relevance } = this.props;

    const { formStaus } = paging;

    const handleRemove = (e) => {
      const id = e.target.value;
      Modal.confirm({
        title: 'Are you sure delete this task?',
        onOk: () => {
          dispatch({
            type: 'config/remove',
            payload: { id },
          });
        }
      });
    }

    const handleRelevance = () => {
      dispatch({
        type: 'config/relevance',
        payload: match.params
      });

      dispatch({
        type: 'relevance/paging',
        payload: match.params
      });

    };

    const handleEdit = (e) => {
      dispatch({ type: 'config/query', payload: { id: e.target.value }, });
    }

    const handleSkipPage = (pageIndex, pageSize) => {
      dispatch({
        type: "config/paging",
        payload: {
          pageIndex,
          pageSize
        }
      })
    }

    const handleSearch = (name) => {
      dispatch({
        type: 'config/search',
        payload: { name, ...match.params },
      });
    }

    const columns = [
      {
        title: 'id',
        dataIndex: 'id',
        key: 'id',
      },
      {
        title: 'name',
        dataIndex: 'name',
        key: 'name',
      },
      {
        title: 'createTime',
        dataIndex: 'createTime',
        key: 'createTime'
      }, {
        title: 'action',
        render: (text, row) => (
          <span>
            <Button type='link' value={row.id} onClick={handleEdit} style={{ marginRight: 16 }}>Edit</Button>
            <Button type='link' value={row.id} onClick={handleRemove}>Delete</Button>
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
            </Col>
            <Col span={6}>
              <Search placeholder="input search text" onSearch={handleSearch} enterButton />
            </Col>
          </Row>
          <Table columns={columns} dataSource={paging.items} pagination={pagination} />
          <Relevance visible={formStaus == 1} match={match}></Relevance>
        </div>
      </PageHeaderWrapper>
    );
  }

}

export default connect(({ config, relevance }) => (
  {
    paging: config,
    relevance
  }))(ProjectList);