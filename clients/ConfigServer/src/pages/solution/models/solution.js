import * as request from "@/services/solution"
import { Modal } from "antd";
import { router } from "umi";

export default {
  state: {
    pageIndex: 1,
    pageSize: 10,
    totalCount: 0,
    items: [],
    params: {},
    model: {}
  },
  reducers: {
    setList(state, { payload: { items } }) {
      return { ...state, items };
    },
    setPaging(state, { payload: { pageIndex, pageSize, totalCount } }) {
      return { ...state, pageIndex, pageSize, totalCount };
    },
    setParams(state, { payload }) {
      return { ...state, params: { ...payload } };
    },
    set(state, { payload }) {
      debugger;
      return { ...state, model: payload };
    }
  },
  effects: {
    *paging({ payload }, { call, put, select }) {
      const params = yield select(({ solution }) => (solution.params));
      const data = yield call(request.queryPaging, { ...payload, ...params });
      yield put({ type: "setList", payload: data });
      yield put({ type: "setPaging", payload: data });
    },
    *search({ payload }, { put }) {
      yield put({ type: "setParams", payload });
      yield put({ type: "paging", payload: {} });
    },
    *add({ payload }, { call, put }) {
      yield call(request.add, payload);
      Modal.success({
        "title": "solution add success",
        "cancelText": "readd",
        "okCancel": true,
        "okText": "go back",
        "onOk": () => {
          router.push("/solution");
        },
        "onCancel":()=> {
           
        }
      });

    },
    *edit({ payload }, { call }) {
      yield call(request.edit, payload);
    },
    *query({ payload }, { call, put }) {
      const model = yield call(request.query, payload);
      yield put({ type: 'set', model });
    }
  },
  subscriptions: {
    setup({ dispatch, history }) {
      history.listen(({ pathname }) => {
        if (pathname === '/solution') {
          dispatch({
            type: 'solution/paging',
          });
        }
      });
    },
  }
};