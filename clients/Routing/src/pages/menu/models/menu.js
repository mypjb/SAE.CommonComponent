import request from "../service";
import { history } from "umi";

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
    items: [],
    model: {},
    formStaus: 0
  },
  reducers: {
    setList(state, { payload: { items } }) {
      return { ...state, items };
    },
    set(state, { payload }) {
      return { ...state, model: payload };
    },
    setFormStaus(state, { payload }) {
      const model = { ...state, formStaus: payload };
      return model;
    }
  },
  effects: {
    *queryList({ payload }, { call, put }) {
      const items = yield call(request.list);
      yield put({ type: "setList", payload: { items } });
    },
    *requestAdd({ payload }, { call, put }) {
      yield put({ type: "set", payload: payload || {} });
      yield put({ type: "setFormStaus", payload: 1 });
    },
    *add({ payload }, { call, put }) {
      yield call(request.add, payload);
      yield put({ type: "setFormStaus", payload: 0 });
      yield put({ type: "set", payload: payload });
      yield put({ type: "queryList", payload: {} });
    },
    *edit({ payload }, { call, put }) {
      yield call(request.edit, payload);
      yield put({ type: "setFormStaus", payload: 0 });
      yield put({ type: "queryList", payload: {} });
    },
    *query({ payload }, { call, put }) {
      const model = yield call(request.query, payload.id);
      yield put({ type: 'set', payload: model });
      yield put({ type: "setFormStaus", payload: 2 });
    },
    *remove({ payload }, { call, put }) {
      const ids = findIds(payload);
      yield call(request.remove, { ids });
      yield put({ type: "queryList", payload: {} });
    }
  },
  subscriptions: {
    setup({ dispatch, history }) {
      history.listen(({ pathname }) => {
        if (pathname === '/menu') {
          dispatch({
            type: 'queryList',
          });
        }
      });
    },
  }
};