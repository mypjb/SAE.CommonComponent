import { Row, Col, Button, Input } from 'antd';
import { connect } from 'umi';
import AddForm from './components/AddForm';
import EditForm from './components/EditForm';
import PagingTable from '@/components/PagingTable';
import { defaultDispatchType, defaultHandler, defaultOperation, Format } from '@/utils/utils';

const { Search } = Input;

export default connect(({ dict }) => (
    {
        dict
    }))((props) => {

        const { dispatch, dict } = props;
        const dispatchType = defaultDispatchType("dict");


        const handleDelete = defaultHandler.delete({ dispatch, dispatchType: dispatchType.delete });

        const handleSearch = defaultHandler.search({ dispatch, dispatchType: dispatchType.search });

        const handleAdd = (parent) => {
            defaultOperation.add({ dispatch, element: AddForm, parent });
        }

        const handleEdit = (row) => {
            defaultOperation.edit({
                dispatch,
                type: dispatchType.find,
                data: row.id,
                element: EditForm
            });
        }

        const handleStatus = (row) => {
            dispatch({
                type: "dict/status",
                payload: {
                    id: row.id,
                    status: Math.abs(row.status - 1)
                }
            })
        };


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
                title: 'parent',
                dataIndex: 'parent',
                render: (parent, row) => {
                    return parent?.name || "--";
                }
            }
            , {
                title: 'type',
                dataIndex: 'type',
                render: Format.dictType
            }, {
                title: 'createTime',
                dataIndex: 'createTime',
                render: Format.date
            }, {
                title: 'action',
                render: (text, row) => (
                    <span>
                        <Button type='link' onClick={handleAdd.bind(row, row)}>add child</Button>
                        <Button type='link' value={row.id} onClick={handleEdit.bind(row, row)} style={{ marginRight: 16 }}>Edit</Button>
                        <Button type='link' onClick={handleDelete.bind(row, { id: row.id })}>Delete</Button>
                    </span>
                )
            }
        ];

        return (<div>
            <Row>
                <Col span={18}>
                    <Button type="primary" onClick={handleAdd.bind(null, null)}>Add</Button>
                </Col>
                <Col span={6}>
                    <Search placeholder="input search text" onSearch={(name) => handleSearch({ name })} enterButton />
                </Col>
            </Row>
            <PagingTable {...props} {...dict} dispatchType={dispatchType.paging} columns={columns} />
        </div>)
    });

