import { PageHeaderWrapper } from '@ant-design/pro-layout';
import React from 'react';
import { Row, Col, Input, Table, Button, Modal } from 'antd';
import { connect,Link} from 'umi';
import AddForm from './components/AddForm';
import EditForm from './components/EditForm';

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
    const { dispatch, project, match } = this.props;

    const { solutionId } = match.params;
    const { formStaus, paging, items } = project;

    const handleRemove = (row) => {
      const id = row.id;
      Modal.confirm({
        title: 'Are you sure delete this task?',
        onOk: () => {
          dispatch({
            type: 'project/remove',
            payload: { id },
          });
        }
      });
    }

    const handleAdd = () => {
      dispatch({ type: 'project/setFormStaus', payload: 1 });
    }

    const handleEdit = (row) => {
      dispatch({ type: 'project/query', payload: { id: row.id }, });
    }

    const handleSkipPage = (pageIndex, pageSize) => {
      dispatch({
        type: "project/paging",
        payload: {
          pageIndex,
          pageSize
        }
      })
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
            <Button type='link' onClick={handleEdit.bind(row,row)} style={{ marginRight: 16 }}>Edit</Button>
            <Button type='link' onClick={handleRemove.bind(row,row)}>Delete</Button>
            <Link to={`/solution/project/config/${row.id}`} >
              <Button type='link'>Config Manage</Button>
            </Link>
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

export default connect(({ project }) => (
  {
    project
  }))(ProjectList);