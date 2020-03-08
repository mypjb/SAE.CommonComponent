import { PageHeaderWrapper } from '@ant-design/pro-layout';
import React from 'react';
import { Row, Col, Input, Table, Button, Modal } from 'antd';
import { connect } from 'dva';

const { Search } = Input;

class Relevance extends React.Component {
  constructor(props) {
    super(props);
    
  }

  render() {
    const { dispatch, paging, match } = this.props;

    const handleSkipPage = (pageIndex, pageSize) => {
      dispatch({
        type: "relevance/paging",
        payload: {
          pageIndex,
          pageSize
        }
      })
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
      },
      {
        title: 'createTime',
        dataIndex: 'createTime',
        key: 'createTime'
      }, {
        title: 'action',
        render: (text, row) => (
          <span>
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
      <Modal forceRender title="add" visible={this.props.visible}  closable={false}>
        <Table columns={columns} dataSource={paging.items} pagination={pagination} />
      </Modal>
    );
  }

}

export default connect(({ relevance }) => (
  {
    paging: relevance
  }))(Relevance);