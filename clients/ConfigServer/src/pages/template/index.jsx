import { PageHeaderWrapper } from '@ant-design/pro-layout';
import React from 'react';
import { Row, Col, Input, Table, Button, Modal } from 'antd';
import { connect } from 'umi';
import styles from './index.less';
import AddForm from './components/AddForm';
import EditForm from './components/EditForm';
import { defaultOperation, defaultDispatchType } from '@/utils/utils';
import PagingTable from '@/components/PagingTable';

const { Search } = Input;

export default connect(({ template }) => (
  {
    template
  }))((props) => {

    const [modal, contextHolder] = Modal.useModal();

    const { dispatch, template } = props;

    const dispatchType = defaultDispatchType("template");

    const handleDelete = (row) => {
      Modal.confirm({
        title: 'Are you sure delete this task?',
        onOk: () => {
          dispatch({
            type: dispatchType.delete,
            payload: { id: row.id },
          });
        }
      });
    }

    const handleAdd = () => {
      defaultOperation.add({ dispatch, element: AddForm }, modal);
    }

    const handleEdit = (row) => {
      defaultOperation.edit({ dispatch, type: dispatchType.find, data: row.id, element: EditForm });
    }



    const handleSearch = (name) => {
      dispatch({
        type: dispatchType.search,
        payload: { name },
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
            <Button type='link' onClick={handleEdit.bind(null, row)} style={{ marginRight: 16 }}>Edit</Button>
            <Button type='link' onClick={handleDelete.bind(null, row)}>Delete</Button>
          </span>
        )
      }
    ];



    return (
      <PageHeaderWrapper className={styles.main}>
        {contextHolder}
        <div>
          <Row>
            <Col span={18}>
              <Button type="primary" onClick={handleAdd}>Add</Button>
            </Col>
            <Col span={6}>
              <Search placeholder="input search text" onSearch={handleSearch} className={styles.search} enterButton />
            </Col>
          </Row>
          <PagingTable {...props} {...template} dispatchType={dispatchType.paging} columns={columns} />
        </div>
      </PageHeaderWrapper>
    );
  });

