import { PageHeaderWrapper } from '@ant-design/pro-layout';
import React from 'react';
import { Row, Col, Button, Modal, Table } from 'antd';
import { connect } from 'umi';
import styles from './index.less';
import AddForm from './components/AddForm';
import EditForm from './components/EditForm';
import PermissionManage from './components/PermissionManage';

import { defaultDispatchType, defaultHandler, defaultOperation } from '@/utils/utils';

export default connect(({ menu }) => (
  {
    menu
  }))((props) => {

    const { dispatch, menu } = props;

    const dispatchType = defaultDispatchType("menu");

    const handleDelete = defaultHandler.delete({ dispatch, dispatchType: dispatchType.delete });

    const handlePermissionManage = (row) => {
      defaultOperation.add({
        dispatch,
        element: PermissionManage,
        menu: row,
        modalProps: {
          width: '80%',
          okCallback: () => {
            dispatch({ type: 'menu/tree' })
          }
        }
      });
    }

    const handleAdd = (model) => {
      defaultOperation.add({
        dispatch,
        element: AddForm,
        model: model,
        modalProps: {
          okCallback: () => {
            dispatch({ type: 'menu/tree' })
          }
        }
      });
    }

    const handleEdit = (row) => {
      defaultOperation.edit({
        dispatch,
        type: dispatchType.find,
        data: row.id,
        element: EditForm,
        modalProps: {
          okCallback: () => {
            dispatch({ type: 'menu/tree' })
          }
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
      }, {
        title: 'path',
        dataIndex: 'path'
      }, {
        title: 'action',
        render: (text, row) => (
          <span>
            <Button type='link' onClick={handleAdd.bind(row, row)}>Add Child</Button>
            <Button type='link' onClick={handlePermissionManage.bind(row, row)}>PermissionManage</Button>
            <Button type='link' value={row.id} onClick={handleEdit.bind(row, row)} style={{ marginRight: 16 }}>Edit</Button>
            <Button type='link' onClick={handleDelete.bind(row, { ids: [row.id] })}>Delete</Button>
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
          </Row>
          <Table rowKey='id' dataSource={menu.tree} childrenColumnName='items' pagination={{ hideOnSinglePage: true }} columns={columns} />
        </div>
      </PageHeaderWrapper>
    );
  });

