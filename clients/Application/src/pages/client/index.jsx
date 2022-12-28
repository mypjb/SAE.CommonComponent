import React from 'react';
import { Row, Col, Button, Input, Modal } from 'antd';
import { connect } from 'umi';
import AddForm from './components/AddForm';
import EditForm from './components/EditForm';
import PagingTable from '@/components/PagingTable';
import { defaultDispatchType, defaultHandler, defaultOperation, Format } from '@/utils/utils';
import RoleManage from './components/RoleManage';

const { Search } = Input;

export default connect(({ client }) => (
    {
        client
    }))((props) => {

        const { dispatch, client, match } = props;

        const dispatchType = defaultDispatchType("client");

        const [modal, contextHolder] = Modal.useModal();

        const handleDelete = defaultHandler.delete({ dispatch, dispatchType: dispatchType.delete });

        const handleSearch = defaultHandler.search({ dispatch, dispatchType: dispatchType.search });

        const handleAdd = (parent) => {
            defaultOperation.add({ dispatch, element: AddForm, parent, appId: match.params.id }, modal);
        }

        const handleEdit = (row) => {
            defaultOperation.edit({
                dispatch,
                type: dispatchType.find,
                data: row.id,
                element: EditForm
            }, modal);
        }

        const handleStatus = (row) => {
            dispatch({
                type: "client/status",
                payload: {
                    id: row.id,
                    status: Math.abs(row.status - 1)
                }
            })
        };

        const handleRefreshSecret = (row) => {
            modal.warning({
                title: "Update secret",
                content: "Note that this action refreshes and downloads the client secret",
                closable: true,
                onOk: function (e) {
                    dispatch({
                        type: "client/refreshSecret",
                        payload: {
                            data: row.id,
                            callback: e
                        }
                    })
                }
            })
        }

        const handleRoleManage = (client) => {
            defaultOperation.add({
                dispatch,
                modalProps: {
                    title: "Role Manage",
                    width: "75%"
                },
                element: RoleManage,
                client
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
                title: 'scopes',
                dataIndex: 'scopes',
                render: Format.scope
            },
            {
                title: 'client id',
                dataIndex: 'id'
            },
            {
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
            },
            {
                title: 'action',
                render: (text, row) => (
                    <span>
                        <Button type='link' onClick={handleRoleManage.bind(row, row)}>Role Manage</Button>
                        <Button type='link' onClick={handleRefreshSecret.bind(row, row)}>Refresh Secret</Button>
                        <Button type='link' value={row.id} onClick={handleEdit.bind(row, row)} style={{ marginRight: 16 }}>Edit</Button>
                        <Button type='link' onClick={handleDelete.bind(row, { id: row.id })}>Delete</Button>
                    </span>
                )
            }
        ];

        return (
            <div>
                {contextHolder}
                <Row>
                    <Col span={18}>
                        <Button type="primary" onClick={handleAdd.bind(null, null)}>Add</Button>
                    </Col>
                    <Col span={6}>
                        <Search placeholder="input search text" onSearch={(name) => handleSearch({ name })} enterButton />
                    </Col>
                </Row>
                <PagingTable {...props} {...client} dispatchType={dispatchType.paging} columns={columns} />
            </div>
        );
    });

