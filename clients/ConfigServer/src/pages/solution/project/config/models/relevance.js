import request from "../service"

export default {
  state: {
    paging: {
      pageIndex: 1,
      pageSize: 10,
      totalCount: 0
    },
    items: [],
    params: {}
  },
  reducers: {
    setList(state, { payload: { items } }) {
      return { ...state, items };
    },
    setPaging(state, { payload: { pageIndex, pageSize, totalCount } }) {
      return { ...state, paging: { pageIndex, pageSize, totalCount } };
    },
    setParams(state, { payload }) {
      return { ...state, params: { ...payload } };
    }
  },
  effects: {
    *paging({ payload }, { call, put, select }) {
      const params = yield select(({ relevance }) => (relevance.params));
      const data = yield call(request.queryRelevance, { ...payload, ...params });
      yield put({ type: "setList", payload: data });
      yield put({ type: "setPaging", payload: data });
    },
    *search({ payload }, { put }) {
      yield put({ type: "setParams", payload });
      yield put({ type: "paging", payload: {} });
    },
    *add({ payload }, { call, put }) {
      yield call(request.relevance, payload);
      yield put({ type: "projectConfig/setFormStaus", payload: 0 });
      yield put({ type: "projectConfig/paging", payload: {} });
    }
  }
};