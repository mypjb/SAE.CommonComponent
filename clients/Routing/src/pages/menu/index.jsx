import { PageHeaderWrapper } from '@ant-design/pro-layout';
import React from 'react';
import { Row, Col, Button, Modal,Table } from 'antd';
import { connect } from 'umi';
import styles from './index.less';
import AddForm from './components/AddForm';
import EditForm from './components/EditForm';
import { defaultDispatchType, defaultHandler, defaultOperation } from '@/utils/utils';

export default connect(({ menu }) => (
  {
    menu
  }))((props) => {

    const { dispatch, menu } = props;

    const dispatchType = defaultDispatchType("menu");

    const handleDelete = defaultHandler.delete({ dispatch, dispatchType: dispatchType.delete });

    //const handleSearch = defaultHandler.search({ dispatch, dispatchType: dispatchType.search });

    const handleAdd = () => {
      defaultOperation.add({ dispatch, element: AddForm });
    }

    const handleEdit = (row) => {
      defaultOperation.edit({
        dispatch,
        type: dispatchType.find,
        data: row.id,
        element: EditForm
      });
    }

    const handleChildAdd = ({ id, name }) => {
      dispatch({ type: 'menu/requestAdd', payload: { parentId: id, parentName: name } });
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
        dataIndex: 'name',
        key: 'name',
      }, {
        title: 'path',
        dataIndex: 'path',
        key: 'path',
      }, {
        title: 'action',
        render: (text, row) => (
          <span>
            <Button type='link' onClick={handleChildAdd.bind(row, row)}>Add Child</Button>
            <Button type='link' value={row.id} onClick={handleEdit.bind(row, row)} style={{ marginRight: 16 }}>Edit</Button>
            <Button type='link' onClick={handleDelete.bind(row, row)}>Delete</Button>
          </span>
        )
      }
    ];
    console.log(props);
    return (
      <PageHeaderWrapper className={styles.main}>
        <div>
          <Row>
            <Col span={18}>
              <Button type="primary" onClick={handleAdd}>Add</Button>
            </Col>
          </Row>
          <Table dataSource={menu.tree} childrenColumnName='items' columns={columns} />
        </div>
      </PageHeaderWrapper>
    );
  });

