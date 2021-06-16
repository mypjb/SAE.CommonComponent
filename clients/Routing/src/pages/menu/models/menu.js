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
    *tree(payload, { call, put }) {
      const { callback } = parsingPayload(payload);
      const data = yield call(request.tree);
      yield put({ type: "setTree", payload: data });
      callback(data);

    }
  },
  subscriptions: {
    setup({ dispatch, history }) {
      history.listen(({ pathname }) => {
        if (pathname === '/menu') {
          dispatch({
            type: 'tree',
          });
        }
      });
    },
  }
};