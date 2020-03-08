import request from "../service";
import templateRequest from "@/services/template";

export default {
  state: {
    pageIndex: 1,
    pageSize: 10,
    totalCount: 0,
    items: [],
    params: {},
    model: {},
    formStaus: 0,
    templates: [],
    template: {}
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
      return { ...state, model: payload };
    },
    setFormStaus(state, { payload }) {
      const model = { ...state, formStaus: payload };
      return model;
    },
    setTemplateList(state, { payload }) {
      return { ...state, templates: payload }
    }
  },
  effects: {
    *paging({ payload }, { call, put, select }) {
      const params = yield select(({ config }) => (config.params));
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
    *query({ payload }, { call, put }) {
      const model = yield call(request.query, payload.id);
      yield put({ type: 'set', payload: model });
      yield put({ type: "setFormStaus", payload: 2 });
    },
    *remove({ payload }, { call, put }) {
      yield call(request.remove, payload.id);
      yield put({ type: 'paging' });
    },
    *queryTemplateList(params, { call, put }) {
      const list = yield call(templateRequest.list);
      yield put({ type: 'setTemplateList', payload: list });
    }
  },
  // subscriptions: {
  //   setup({ dispatch, history }) {
  //     // history.listen(({ pathname }) => {
  //     //   if (pathname === '/solution/config') {
  //     //     dispatch({
  //     //       type: 'paging',
  //     //     });
  //     //   }
  //     // });
  //   },
  // }
};