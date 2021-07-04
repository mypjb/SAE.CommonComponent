import { PageHeaderWrapper } from '@ant-design/pro-layout';
import React from 'react';
import { Row, Col, Button, Modal, Table, Input } from 'antd';
import { connect } from 'umi';
import styles from './index.less';
import AddForm from './components/AddForm';
import EditForm from './components/EditForm';
import PermissionManage from './components/PermissionManage';
import MenuManage from './components/MenuManage';
import PagingTable from '@/components/PagingTable';
import { defaultDispatchType, defaultHandler, defaultOperation, Format } from '@/utils/utils';

const { Search } = Input;

export default connect(({ role }) => (
    {
        role
    }))((props) => {

        const { dispatch, role } = props;

        const dispatchType = defaultDispatchType("role");


        const handleDelete = defaultHandler.delete({ dispatch, dispatchType: dispatchType.delete });

        const handleSearch = defaultHandler.search({ dispatch, dispatchType: dispatchType.search });

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
                type: "role/status",
                payload: {
                    id: row.id,
                    status: Math.abs(row.status - 1)
                }
            })
        };

        const handlePermissionManage = (role) => {
            defaultOperation.add({
                dispatch,
                modalProps: {
                    title: "Permission Manage",
                    width: "75%"
                },
                element: PermissionManage,
                role
            });
        }

        const handleMenuManage = (role) => {
            defaultOperation.add({
                dispatch,
                modalProps: {
                    title: "Menu Manage",
                    width: "75%"
                },
                element: MenuManage,
                role
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
                title: 'descriptor',
                dataIndex: 'descriptor'
            }, {
                title: 'createTime',
                dataIndex: 'createTime',
                render: Format.date
            }, {
                title: 'action',
                render: (text, row) => (
                    <span>
                        <Button type='link' onClick={handlePermissionManage.bind(row, row)}>Permission Manage</Button>
                        <Button type='link' onClick={handleMenuManage.bind(row, row)}>Menu Manage</Button>
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
                        <Col span={6}>
                            <Search placeholder="input search text" onSearch={(name) => handleSearch({ name })} className={styles.search} enterButton />
                        </Col>
                    </Row>
                    <PagingTable {...props} {...role} dispatchType={dispatchType.paging} columns={columns} />
                </div>
            </PageHeaderWrapper>
        );
    });

