import { Table } from "antd";
import { useState } from "react";


export default (props) => {

    const { dispatch, dispatchType } = props;
    const { paging, items } = props;

    let handleSkipPage;

    if (props.handleSkipPage) {
        handleSkipPage = props.handleSkipPage;
    } else {
        handleSkipPage = (pageIndex, pageSize) => {
            dispatch({
                type: dispatchType,
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
    }


    const pagination = {
        current: paging.pageIndex,
        total: paging.totalCount,
        pageSize: paging.pageSize,
        onChange: handleSkipPage
    };
    return (<Table {...props} dataSource={items} pagination={pagination} />);
}