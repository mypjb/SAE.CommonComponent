import PagingTable from '@/components/PagingTable';
import {
  defaultDispatchType,
  defaultHandler,
  defaultOperation,
  Format,
} from '@/utils/utils';
import { Button, Col, Input, Row } from 'antd';
import { useEffect } from 'react';
import { connect } from 'umi';
import AddForm from './components/AddForm';
import EditForm from './components/EditForm';

const { Search } = Input;

export default connect(({ rule }) => ({
  rule,
}))((props) => {
  const { dispatch, rule } = props;

  const dispatchType = defaultDispatchType('rule');

  const handleDelete = defaultHandler.delete({
    dispatch,
    dispatchType: dispatchType.delete,
  });

  const handleSearch = defaultHandler.search({
    dispatch,
    dispatchType: dispatchType.search,
  });

  useEffect(() => {
    handleSearch();
  }, []);

  const handleAdd = () => {
    defaultOperation.add({ dispatch, element: AddForm });
  };

  const handleEdit = (row) => {
    defaultOperation.edit({
      dispatch,
      type: dispatchType.find,
      data: row.id,
      element: EditForm,
    });
  };

  const columns = [
    {
      title: 'serial number',
      dataIndex: 'id',
      render: (text, record, index) => {
        return index + 1;
      },
    },
    {
      title: 'name',
      dataIndex: 'name',
    },
    {
      title: 'description',
      dataIndex: 'description',
    },
    {
      title: 'operator',
      dataIndex: 'symbol',
      render: (symbol, row) => {
        if (row.right) {
          return row.left + symbol + row.right;
        } else {
          return row.symbol + row.left;
        }
      },
    },
    {
      title: 'createTime',
      dataIndex: 'createTime',
      render: Format.date,
    },
    {
      title: 'action',
      render: (text, row) => (
        <span>
          <Button
            type="link"
            value={row.id}
            onClick={handleEdit.bind(row, row)}
            style={{ marginRight: 16 }}
          >
            Edit
          </Button>
          <Button
            type="link"
            onClick={handleDelete.bind(row, { ids: [row.id] })}
          >
            Delete
          </Button>
        </span>
      ),
    },
  ];

  return (
    <div>
      <Row>
        <Col span={18}>
          <Button type="primary" onClick={handleAdd}>
            Add
          </Button>
        </Col>
        <Col span={6}>
          <Search placeholder="input search text" onSearch={(name) => handleSearch({ name })} enterButton />
        </Col>
      </Row>
      <PagingTable
        {...props}
        {...rule}
        dispatchType={dispatchType.paging}
        columns={columns}
      />
    </div>
  );
});
