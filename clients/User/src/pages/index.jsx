import React, { useEffect } from 'react';
import { Row, Col, Button, Modal, Table, Input } from 'antd';
import { connect } from 'umi';
import AddForm from './components/AddForm';
import EditForm from './components/EditForm';
import RoleManage from './components/RoleManage';
import PagingTable from '@/components/PagingTable';
import { defaultDispatchType, defaultHandler, defaultOperation, Format } from '@/utils/utils';

const { Search } = Input;

export default connect(({ user }) => (
    {
        user
    }))((props) => {

        const { dispatch, user } = props;

        const dispatchType = defaultDispatchType("user");

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
                type: "user/status",
                payload: {
                    id: row.id,
                    status: Math.abs(row.status - 1)
                }
            })
        };

        const handleRoleManage = (user) => {
            defaultOperation.add({
                dispatch,
                modalProps: {
                    title: "Role Manage",
                    width: "75%"
                },
                element: RoleManage,
                user
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
                        <Button type='link' onClick={handleRoleManage.bind(row, row)}>Role Manage</Button>
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
                        <Button type="primary" onClick={handleAdd}>Add</Button>
                    </Col>
                    <Col span={6}>
                        <Search placeholder="input search text" onSearch={(name) => handleSearch({ name })} enterButton />
                    </Col>
                </Row>
                <PagingTable {...props} {...user} dispatchType={dispatchType.paging} columns={columns} />
            </div>
        );
    });

