import request from "../service";
import { history } from "umi";
import { defaultModel, parsingPayload } from '@/utils/utils'

const findIds = function (row) {
  let array = [row.id];
  if (row.items && row.items.length) {
    row.items.forEach(element => {
      const items = findIds(element);
      array = array.concat(items);
    });
  }
  return array;
}

export default {
  namespace: "menu",
  state: {
    ...defaultModel.state,
    tree: []
  },
  reducers: {
    ...defaultModel.reducers,
    setTree(state, { payload }) {
      return { ...state, tree: payload };
    }
  },
  effects: {
    ...defaultModel.effects({ request, name: "menu" }),
    *delete({ payload }, { call, put }) {
      yield call(request.delete, payload);
      yield put({ type: 'tree' });
    },
    *tree(payload, { call, put }) {
      const { callback } = parsingPayload(payload);
      const data = yield call(request.tree);
      yield put({ type: "setTree", payload: data });
      callback(data);
    }
  }
};