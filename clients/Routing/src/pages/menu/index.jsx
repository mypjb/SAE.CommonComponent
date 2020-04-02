import { PageHeaderWrapper } from '@ant-design/pro-layout';
import React from 'react';
import { Row, Col, Input, Table, Button, Modal } from 'antd';
import { connect } from 'umi';
import styles from './index.less';
import AddForm from './components/AddForm';
import EditForm from './components/EditForm';

const { Search } = Input;

export default connect(({ menu }) => (
  {
    menu
  }))(({ dispatch, menu }) => {
    const { formStaus, paging, items } = menu;

    const handleRemove = (row) => {
      Modal.confirm({
        title: 'Are you sure delete this task?',
        onOk: () => {
          dispatch({
            type: 'menu/remove',
            payload: row,
          });
        }
      });
    }

    const handleAdd = () => {
      dispatch({ type: 'menu/requestAdd' });
    }

    const handleChildAdd = ({ id, name }) => {
      
      dispatch({ type: 'menu/requestAdd', payload: { parentId: id, parentName: name } });
    }

    const handleEdit = (e) => {
      dispatch({ type: 'menu/query', payload: { id: e.target.value }, });
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
      }, {
        title: 'path',
        dataIndex: 'path',
        key: 'path',
      }, {
        title: 'action',
        render: (text, row) => (
          <span>
            <Button type='link' onClick={handleChildAdd.bind({}, row)}>Add Child</Button>
            <Button type='link' value={row.id} onClick={handleEdit} style={{ marginRight: 16 }}>Edit</Button>
            <Button type='link' onClick={handleRemove.bind({},row)}>Delete</Button>
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
          <Table columns={columns} childrenColumnName='items'  dataSource={items} />
          <AddForm visible={formStaus === 1} />
          <EditForm visible={formStaus === 2} />
        </div>
      </PageHeaderWrapper>
    );
  });

