import { parse } from 'querystring';
import pathRegexp from 'path-to-regexp';
import { history } from "umi";

export const validatorJson = (rule, value) => {
  if (value) {
    try {
      eval(`(${value})`);
    } catch (e) {
      console.error(e);
      return Promise.reject('this is json invalid');
    }
  }
  return Promise.resolve();
}

export const handleFormat = function ({ form, fieldName }, e) {
  const value = e.target.value;
  if (value) {
    try {
      const json = eval('(' + value + ')');
      const data = {};
      data[fieldName] = JSON.stringify(json, null, 4);
      form.setFieldsValue(data);
    } catch { }
  }
};

//Default Model
export const defaultModel = {
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
    },
    set(state, { payload }) {
      return { ...state, model: payload };
    }
  },
  effects: function (props) {
    const { request, name } = props;
    const stateName = name;
    return {
      *add({ payload }, { call, put }) {
        yield call(request.add, payload);
        yield put({ type: "setFormStaus", payload: 0 });
        yield put({ type: "set", payload: {} });
        if (props.addSuccessUrl) {
          history.push(addSuccessUrl);
        }
      },
      *delete({ payload }, { call, put }) {
        yield call(request.delete, payload);
        yield put({ type: 'paging' });
      },
      *edit({ payload }, { call, put }) {
        const { callback, data } = payload;
        yield call(request.edit, data);
        yield put({ type: "paging", payload: {} });
        if (callback) {
          callback();
        };
      },
      *find({ payload }, { put, call }) {
        const { callback, data } = payload;
        const model = yield call(request.find, data);
        if (callback) {
          callback(model);
        };
      },
      *paging({ payload }, { call, put, select }) {
        const params = yield select((globalStatus) => (globalStatus[stateName].params));
        const data = yield call(request.queryPaging, { ...payload, ...params });
        yield put({ type: "setList", payload: data });
        yield put({ type: "setPaging", payload: data });
      },
      *search({ payload }, { put }) {
        yield put({ type: "setParams", payload });
        yield put({ type: "paging", payload: {} });
      },
    };
  }
}