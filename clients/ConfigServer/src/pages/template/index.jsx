import { PageHeaderWrapper } from '@ant-design/pro-layout';
import React from 'react';
import { Row, Col, Input, Table, Button, Modal } from 'antd';
import { connect } from 'umi';
import styles from './index.less';
import AddForm from './components/AddForm';
import EditForm from './components/EditForm';

const { Search } = Input;

export default connect(({ template }) => (
  {
    template
  }))(({ dispatch, template }) => {
    const { formStaus, paging, items } = template;

    const handleRemove = (row) => {
      Modal.confirm({
        title: 'Are you sure delete this task?',
        onOk: () => {
          dispatch({
            type: 'template/remove',
            payload: { id:row.id },
          });
        }
      });
    }

    const handleAdd = () => {
      dispatch({ type: 'template/setFormStaus', payload: 1 });
    }

    const handleEdit = (row) => {
      dispatch({ type: 'template/query', payload: { id: row.id }, });
    }

    const handleSkipPage = (pageIndex, pageSize) => {
      dispatch({
        type: "template/paging",
        payload: {
          pageIndex,
          pageSize
        }
      })
    }

    const handleSearch = (name) => {
      dispatch({
        type: 'template/search',
        payload: { name },
      });
    }

    const columns = [
      {
        title: 'serial number',
        dataIndex: 'id',
        key: 'id',
        render:(text,record,index)=>{
          return index+1;
        }
      },
      {
        title: 'name',
        dataIndex: 'name',
        key: 'name',
      },
      {
        title: 'format',
        dataIndex: 'format',
        key: 'format',
        ellipsis: true
      }, {
        title: 'createTime',
        dataIndex: 'createTime',
        key: 'createTime'
      }, {
        title: 'action',
        render: (text, row) => (
          <span>
            <Button type='link' onClick={handleEdit.bind(null,row)} style={{ marginRight: 16 }}>Edit</Button>
            <Button type='link' onClick={handleRemove.bind(null,row)}>Delete</Button>
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
      <PageHeaderWrapper className={styles.main}>
        <div>
          <Row>
            <Col span={18}>
              <Button type="primary" onClick={handleAdd}>Add</Button>
            </Col>
            <Col span={6}>
              <Search placeholder="input search text" onSearch={handleSearch} className={styles.search} enterButton />
            </Col>
          </Row>
          <Table columns={columns} dataSource={items} pagination={pagination} />
          <AddForm visible={formStaus === 1} />
          <EditForm visible={formStaus === 2} />
        </div>
      </PageHeaderWrapper>
    );
  });

