import { PageHeaderWrapper } from '@ant-design/pro-layout';
import React from 'react';
import { Row, Col, Input, Table, Button, Modal } from 'antd';
import { connect } from 'dva';
import AddForm from './components/AddForm';
import EditForm from './components/EditForm';

const { Search } = Input;

class configList extends React.Component {
  constructor(props) {
    super(props);
    props.dispatch({
      type: "config/search",
      payload: props.match.params
    });

    props.dispatch({
      type: "config/queryTemplateList"
    });
  }


  render() {
    const { dispatch, config, match } = this.props;

    const { formStaus, paging, items } = config;

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

    const handleAdd = () => {
      dispatch({ type: 'config/setFormStaus', payload: 1 });
    }

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
        title: 'content',
        dataIndex: 'content',
        key: 'content',
        ellipsis: true
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
              <Button type="primary" onClick={handleAdd}>Add</Button>
            </Col>
            <Col span={6}>
              <Search placeholder="input search text" onSearch={handleSearch} enterButton />
            </Col>
          </Row>
          <Table columns={columns} dataSource={items} pagination={pagination} />
          <AddForm visible={formStaus === 1} />
          <EditForm visible={formStaus === 2} />
        </div>
      </PageHeaderWrapper>
    );
  }

}

export default connect(({ config }) => (
  {
    config
  }))(configList);