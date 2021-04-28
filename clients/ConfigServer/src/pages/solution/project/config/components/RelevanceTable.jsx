import { useEffect, useState } from 'react';
import PagingTable from "@/components/PagingTable";
import { defaultModel, defaultState } from "@/utils/utils";

export default (props) => {

  const { dispatch, projectId } = props;

  const [state, setState] = useState(defaultState);

  let configIds = [];

  useEffect(() => {
    dispatch({
      type: "projectConfigRelevance/paging",
      payload: {
        pageIndex: state.paging.pageIndex,
        callback:setState
      }
    });
  }, [state.paging.pageIndex]);



  const columns = [
    {
      title: 'serial number',
      dataIndex: 'id',
      render: (text, row, index) => {
        return index + 1;
      }
    },
    {
      title: 'name',
      dataIndex: 'name',
    },
    {
      title: 'content',
      dataIndex: 'content',
      ellipsis: true
    },
    {
      title: 'createTime',
      dataIndex: 'createTime',
    }
  ];

  const rowSelectionOption = {
    onChange: (selectedRowKeys, selectedRows) => {
      configIds = selectedRows.map(s => (s.id));
    }
  }

  return (
    <PagingTable {...props} {...state} columns={columns} rowSelection={rowSelectionOption} />
  );
};