import React from 'react';
import { Table, Modal } from 'antd';
import { connect } from 'umi';


class Relevance extends React.Component {
  constructor(props) {
    super(props);
  }

  render() {

    let configIds = [];

    const { dispatch, relevance } = this.props;
    const { paging, items } = relevance;
    const { projectId } = relevance.params;
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
        ellipsis: true
      },
      {
        title: 'createTime',
        dataIndex: 'createTime',
        key: 'createTime'
      }
    ];

    const pagination = {
      current: paging.pageIndex,
      total: paging.totalCount,
      size: paging.pageSize,
      onChange: handleSkipPage
    };

    const rowSelectionOption = {
      onChange: (selectedRowKeys, selectedRows) => {
        configIds = selectedRows.map(s => (s.id));
      }
    }

    const handleOk = () => {
      dispatch({
        type: "relevance/add",
        payload: {
          projectId,
          configIds
        }
      });
    };

    const handleCancel = () => {
      dispatch({ type: 'projectConfig/setFormStaus', payload: 0 });
    };

    return (
      <Modal forceRender width="70%" title="add" onOk={handleOk} onCancel={handleCancel} visible={this.props.visible} closable={false}>
        <Table columns={columns} rowSelection={rowSelectionOption} dataSource={items} pagination={pagination} />
      </Modal>
    );
  }
}

export default connect(({ relevance }) => (
  {
    relevance
  }))(Relevance);