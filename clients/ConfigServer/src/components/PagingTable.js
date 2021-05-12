import { Table } from "antd";
import { useState } from "react";


export default (props) => {

    const { dispatch, type } = props;
    const [state, setState] = useState({ paging: props.paging, items: props.items });
    const { paging, items } = state;
    const handleSkipPage = (pageIndex, pageSize) => {
        dispatch({
            type: type,
            payload: {
                data: {
                    pageIndex,
                    pageSize
                },
                callback: (data) => {
                    setState(data);
                }
            }
        })
    }

    const pagination = {
        current: paging.pageIndex,
        total: paging.totalCount,
        size: paging.pageSize,
        onChange: handleSkipPage
    };
    return (<Table {...props} dataSource={items} pagination={pagination} />);
}