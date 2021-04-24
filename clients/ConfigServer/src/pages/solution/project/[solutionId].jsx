import { PageHeaderWrapper } from '@ant-design/pro-layout';
import React from 'react';
import { Row, Col, Input, Table, Button, Modal } from 'antd';
import { connect, Link } from 'umi';
import AddForm from './components/AddForm';
import EditForm from './components/EditForm';
import { defaultOperation } from '@/utils/utils';
import PagingTable from '@/components/PagingTable';

const { Search } = Input;

class ProjectList extends React.Component {
  constructor(props) {
    super(props);
    props.dispatch({
      type: "project/search",
      payload: props.match.params
    });
  }


  render() {
    const { dispatch, match, project } = this.props;


    const handleDelete = (row) => {
      const id = row.id;
      Modal.confirm({
        title: 'Are you sure delete this task?',
        onOk: () => {
          dispatch({
            type: 'project/delete',
            payload: { id },
          });
        }
      });
    }

    const handleAdd = () => {
      defaultOperation.add({ dispatch, element: AddForm, solutionId: match.params.solutionId });
    }

    const handleEdit = (row) => {
      defaultOperation.edit({ dispatch, type: 'project/find', data: row.id, element: EditForm });
    }

    const handleSearch = (name) => {
      dispatch({
        type: 'project/search',
        payload: { name, ...match.params },
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
        title: 'version',
        dataIndex: 'version',
        key: 'version',
      },
      {
        title: 'createTime',
        dataIndex: 'createTime',
        key: 'createTime'
      }, {
        title: 'action',
        render: (text, row) => (
          <span>
            <Button type='link' onClick={handleEdit.bind(row, row)} style={{ marginRight: 16 }}>Edit</Button>
            <Button type='link' onClick={handleDelete.bind(row, row)}>Delete</Button>
            <Link to={`/solution/project/config/${row.id}`} >
              <Button type='link'>Config Manage</Button>
            </Link>
          </span>
        )
      }
    ];

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
          <PagingTable columns={columns} {...this.props} {...project} />
        </div>
      </PageHeaderWrapper>
    );
  }

}

export default connect(({ project }) => (
  {
    project
  }))(ProjectList);