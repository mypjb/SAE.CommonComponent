import request from "../roleMenuService";
import { defaultModel, parsingPayload } from '@/utils/utils'

export default {
  namespace: "roleMenu",
  state: {
    ...defaultModel.state,
    tree: []
  },
  reducers: {
    ...defaultModel.reducers,
  },
  effects: {
    *add({ payload }, { call, put }) {

      const { callback, data } = parsingPayload(payload);

      yield call(request.add, data);

      callback();
    },
    *delete({ payload }, { call, put }) {
      const { callback, data } = parsingPayload(payload);
      yield call(request.delete, data);
      callback();
    },
    *edit({ payload }, { call, put }) {
      const { callback, data } = parsingPayload(payload);

      if (data.reference && data.reference.menuIds.length) {
        yield put({
          type: "add",
          payload: data.reference
        });
      }

      if (data.unReference && data.unReference.menuIds.length) {
        yield put({
          type: "delete",
          payload: data.unReference
        });
      }

      callback();
    },
    *tree({ payload }, { call, put }) {
      const { callback, data } = parsingPayload(payload);
      const treeData = yield call(request.tree, data);
      callback(treeData);
    }
  }
};