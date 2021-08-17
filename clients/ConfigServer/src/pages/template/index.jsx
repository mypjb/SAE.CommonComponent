import { PageHeaderWrapper } from '@ant-design/pro-layout';
import React from 'react';
import { Row, Col, Input, Table, Button, Modal } from 'antd';
import { connect } from 'umi';
import styles from './index.less';
import AddForm from './components/AddForm';
import EditForm from './components/EditForm';
import { defaultOperation, defaultDispatchType, defaultHandler } from '@/utils/utils';
import PagingTable from '@/components/PagingTable';

const { Search } = Input;

export default connect(({ template }) => (
  {
    template
  }))((props) => {
    const { dispatch, template } = props;

    const dispatchType = defaultDispatchType("template");


    const handleDelete = defaultHandler.delete({ dispatch, dispatchType: dispatchType.delete });

    const handleSearch = defaultHandler.search({ dispatch, dispatchType: dispatchType.search });

    const handleAdd = () => {
      defaultOperation.add({ dispatch, element: AddForm });
    }

    const handleEdit = (row) => {
      defaultOperation.edit({
        dispatch,
        type: dispatchType.find,
        data: row.id,
        element: EditForm,
        onOk: (e) => {
          
        }
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
        title: 'name',
        dataIndex: 'name'
      },
      {
        title: 'format',
        dataIndex: 'format',
        ellipsis: true
      }, {
        title: 'createTime',
        dataIndex: 'createTime'
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
        <div>
          <Row>
            <Col span={18}>
              <Button type="primary" onClick={handleAdd}>Add</Button>
            </Col>
            <Col span={6}>
              <Search placeholder="input search text" onSearch={(name) => handleSearch({ name })} className={styles.search} enterButton />
            </Col>
          </Row>
          <PagingTable {...props} {...template} dispatchType={dispatchType.paging} columns={columns} />
        </div>
      </PageHeaderWrapper>
    );
  });

