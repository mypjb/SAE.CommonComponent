import React, { useEffect } from 'react';
import { Row, Col, Button, Modal, Table, Input } from 'antd';
import { connect } from 'umi';
import EditForm from './components/EditForm';
import PagingTable from '@/components/PagingTable';
import { defaultDispatchType, defaultHandler, defaultOperation, Format } from '@/utils/utils';

const { Search } = Input;

export default connect(({ plugin }) => (
    {
        plugin
    }))((props) => {
        console.log({props});
        const { dispatch, plugin } = props;

        const dispatchType = defaultDispatchType("plugin");

        const handleDelete = defaultHandler.delete({ dispatch, dispatchType: dispatchType.delete });

        const handleSearch = defaultHandler.search({ dispatch, dispatchType: dispatchType.search });

        useEffect(() => {
            handleSearch();
          }, []);

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

        const handleStatus = (row) => {
            dispatch({
                type: "plugin/status",
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
                title: 'entry',
                dataIndex: 'entry'
            },
            {
                title: 'path',
                dataIndex: 'path'
            }
            , {
                title: 'status',
                dataIndex: 'status',
                render: (status, row) => {
                    return Format.status(status, { onClick: handleStatus.bind(row, row) });
                }
            },
            {
                title: 'createTime',
                dataIndex: 'createTime',
                render: Format.date
            }, {
                title: 'action',
                render: (text, row) => (
                    <span>
                        <Button type='link' value={row.id} onClick={handleEdit.bind(row, row)} style={{ marginRight: 16 }}>Edit</Button>
                        <Button type='link' disabled onClick={handleDelete.bind(row, { id: [row.id] })}>Delete</Button>
                    </span>
                )
            }
        ];

        return (
            <div>
                <Row>
                    <Col span={18}>

                    </Col>
                    <Col span={6}>
                        <Search placeholder="input search text" onSearch={(name) => handleSearch({ name })} enterButton />
                    </Col>
                </Row>
                <PagingTable {...props} {...plugin} dispatchType={dispatchType.paging} columns={columns} />
            </div>
        );
    });

