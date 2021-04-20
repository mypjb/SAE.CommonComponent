import request from "../service";

export default {
  state: {
    paging: {
      pageIndex: 1,
      pageSize: 10,
      totalCount: 0,
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
    *paging({ payload }, { call, put, select }) {
      const params = yield select(({ project }) => (project.params));
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
      yield put({ type: "setFormStaus", payload: 0 });
      yield put({ type: "set", payload: {} });
      yield put({ type: "paging", payload: {} });
    },
    *edit({ payload }, { call, put }) {
      yield call(request.edit, payload);
      yield put({ type: "setFormStaus", payload: 0 });
      yield put({ type: "paging", payload: {} });
    },
    *find({ payload }, { call, put }) {
      const { callback, id } = payload;
      //delete payload.callback;
      const model = yield call(request.find,id);
      callback(model);
    },
    *remove({ payload }, { call, put }) {
      yield call(request.remove, payload.id);
      yield put({ type: 'paging' });
    }
  }
};