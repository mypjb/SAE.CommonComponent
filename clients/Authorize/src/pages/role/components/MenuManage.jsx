import { PageHeaderWrapper } from '@ant-design/pro-layout';
import { MenuOutlined } from '@ant-design/icons';
import React, { useState, useEffect } from 'react';
import { Tree } from 'antd';
import { defaultDispatchType, } from '@/utils/utils';

const treeTransform = (treeData) => {
  const array = [];
  if (treeData && treeData.length) {
    for (let index = 0; index < treeData.length; index++) {
      const element = treeData[index];
      const treeItem = {
        title: element.name,
        key: element.id,
        children: treeTransform(element.items)
      };
      array.push(treeItem);
    }
  }

  return array;
}

export default (props) => {

  const { dispatch, role } = props;

  const menuIds = role.menuIds || [];

  const dispatchType = defaultDispatchType("roleMenu");

  const [state, setState] = useState({
    treeData: [],
    checkedKeys: menuIds
  });

  const loadTree = () => {
    dispatch({
      type: "roleMenu/tree",
      payload: {
        callback: (data) => {
          const treeData = treeTransform(data);
          console.log({ data, treeData });
          setState({ ...state, treeData });
        }
      }
    })
  }

  const handleCheck = (checkedKeys) => {
    setState({ ...state, checkedKeys });
  }

  useEffect(() => {
    loadTree();
  }, []);

  props.okCallback((close) => {
    const checkedKeys = state.checkedKeys;
    dispatch({
      type: dispatchType.edit,
      payload: {
        reference: {
          id: role.id,
          menuIds: checkedKeys.filter(id => {
            return menuIds.indexOf(id) === -1;
          })
        },
        unReference: {
          id: role.id,
          menuIds: menuIds.filter(id => {
            return checkedKeys.indexOf(id) === -1;
          })
        },
        callback: close
      }
    });
    return false;
  });

  return (
    <PageHeaderWrapper >
      <div>
        <Tree
          checkable
          checkedKeys={state.checkedKeys}
          defaultExpandAll={true}
          icon={<MenuOutlined />}
          showIcon={true}
          onCheck={handleCheck}
          treeData={state.treeData}>
        </Tree>
      </div>
    </PageHeaderWrapper >
  );
};

