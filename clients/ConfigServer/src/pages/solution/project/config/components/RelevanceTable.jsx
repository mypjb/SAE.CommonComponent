import { useEffect, useState } from 'react';
import PagingTable from "@/components/PagingTable";
import { defaultModel, defaultState } from "@/utils/utils";
import { useModel } from 'umi';
import { Select, Row, Col } from 'antd';

const { Option } = Select;

export default (props) => {

  const { environmentData } = useModel("environment", model => ({ environmentData: model.state }));

  const environmentOptions = environmentData.map(data => <Option value={data.id} data={data}>{data.name}</Option>);

  const defaultEnvId = environmentData.length ? environmentData[0].id : "";

  const { dispatch, projectId } = props;

  const [state, setState] = useState({ ...defaultState, params: { projectId, defaultEnvId } });
  const { paging } = state;

  let configIds = [];

  const handleSkipPage = (pageIndex, pageSize) => {
    dispatch({
      type: "projectConfigRelevance/paging",
      payload: {
        pageIndex,
        pageSize,
        ...state.params,
        callback: (data) => {
          console.log({ state, data });
          setState({ ...data, params: state.params });
        }
      }
    });
  };

  const handleChange = (environmentId) => {
    setState({ ...defaultState, params: { ...state.params, environmentId } });
  }

  useEffect(() => {
    handleSkipPage(paging.pageIndex, paging.pageSize);
  }, [state.params.environmentId]);



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
      ellipsis: true
    }
  ];

  const rowSelectionOption = {
    onChange: (selectedRowKeys, selectedRows) => {
      configIds = selectedRows.map(s => (s.id));
    }
  }

  return (
    <div>
      <Row>
        <Col span={18}>

        </Col>
        <Col span={6}>
          <Select style={{ width: '100%' }} defaultValue={defaultEnvId} onChange={handleChange}>
            {environmentOptions}
          </Select>
        </Col>
      </Row>
      <PagingTable {...props}
        {...state}
        handleSkipPage={handleSkipPage}
        columns={columns}
        rowSelection={rowSelectionOption} />
    </div>);
};