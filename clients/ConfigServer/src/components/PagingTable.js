import { Table } from "antd";


export default (props) => {

    const { items, paging, dispatch, type } = props;
    const handleSkipPage = (pageIndex, pageSize) => {
        dispatch({
            type: type,
            payload: {
                pageIndex,
                pageSize
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