import { useEffect, useState } from 'react';
import PagingTable from "@/components/PagingTable";
import { defaultModel, defaultState } from "@/utils/utils";

export default (props) => {

  const { dispatch, projectId } = props;

  const [state, setState] = useState(defaultState);
  const { paging } = state;

  let configIds = [];

  const handleSkipPage = (pageIndex, pageSize) => {
    dispatch({
      type: "projectConfigRelevance/paging",
      payload: {
        pageIndex,
        pageSize,
        projectId,
        callback: (data) => {
          console.log({ state, data });
          setState(data);
        }
      }
    });
  };

  useEffect(() => {
    handleSkipPage(paging.pageIndex, paging.pageSize);
  }, [paging.pageIndex]);
 


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
      title: 'version',
      dataIndex: 'version',
    },
    {
      title: 'content',
      dataIndex: 'content',
      ellipsis:true
    }
  ];

  const rowSelectionOption = {
    onChange: (selectedRowKeys, selectedRows) => {
      configIds = selectedRows.map(s => (s.id));
    }
  }

  return (
    <PagingTable {...props}
      {...state}
      handleSkipPage={handleSkipPage}
      columns={columns}
      rowSelection={rowSelectionOption} />
  );
};