import request from "../service"

export default {
  state: {
    paging: {
      pageIndex: 1,
      pageSize: 10,
      totalCount: 0
    },
    items: [],
    params: {},
    model: {},
    formStaus: 0
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
    *refresh(o, { put, select }) {
      const paging = yield select(({ projectConfig }) => (projectConfig.paging));
      yield put({ type: "paging", payload: paging });
    },
    *paging({ payload }, { call, put, select }) {

      const params = yield select(({ projectConfig }) => (projectConfig.params));
      const data = yield call(request.queryPaging, { ...payload, ...params });

      yield put({ type: "setList", payload: data });
      yield put({ type: "setPaging", payload: data });
    },
    *search({ payload }, { put }) {
      yield put({ type: "setParams", payload });
      yield put({ type: "paging", payload: {} });
    },
    *relevance({ payload }, { put }) {
      yield put({ type: "setFormStaus", payload: 1 });
      yield put({ type: "relevance/search", payload });
    },
    *query({ payload }, { put, call }) {
      const data = yield call(request.query, payload);
      yield put({ type: "set", payload:data });
      yield put({ type: "setFormStaus", payload: 2 });
    },
    *edit({ payload }, { call, put }) {
      yield call(request.edit, payload);
      yield put({ type: "setFormStaus", payload: 0 });
      yield put({ type: "paging", payload: {} });
    },
    *remove({ payload }, { call, put }) {
      yield call(request.remove, payload);
      yield put({ type: 'paging' });
    }
  }
};