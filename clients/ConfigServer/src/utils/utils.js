import { parse } from 'querystring';
import pathRegexp from 'path-to-regexp';
import { history } from "umi";
import FormModal from "@/components/FormModal";

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
};

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

export const defaultState = {
  paging: {
    pageIndex: 1,
    pageSize: 10,
    totalCount: 0
  },
  items: [],
  params: {},
};


const parsingPayload = (payload) => {
  let model = {};
  if (payload && payload.callback) {
    model.callback = payload.callback;
  } else {
    model.callback = () => { };
  }
  if (payload && payload.data) {
    model.data = payload.data;
  } else {
    model.data = {};
  }
  console.log({ model });
  return model;
};

//Default Model
export const defaultModel = {
  state: {
    ...defaultState
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

        const { callback, data } = parsingPayload(payload);

        yield call(request.add, data);

        if (callback) {
          callback();
        };
        yield put({ type: "paging", payload: {} });
      },
      *delete({ payload }, { call, put }) {
        yield call(request.delete, payload);
        yield put({ type: 'paging' });
      },
      *edit({ payload }, { call, put }) {
        const { callback, data } = parsingPayload(payload);
        yield call(request.edit, data);
        yield put({ type: "paging", payload: {} });
        if (callback) {
          callback();
        };
      },
      *find({ payload }, { put, call }) {
        const { callback, data } = parsingPayload(payload);
        const model = yield call(request.find, data);
        if (callback) {
          callback(model);
        };
      },
      *paging({ payload }, { call, put, select }) {

        const { callback, data } = parsingPayload(payload);

        const params = yield select((globalStatus) => (globalStatus[stateName].params));

        const paging = yield call(request.queryPaging, { ...data, ...params });

        yield put({ type: "setList", payload: paging });

        yield put({ type: "setPaging", payload: paging });

        if (callback) {
          const state = yield select((globalStatus) => (globalStatus[stateName]));
          callback(state);
        }
      },
      *search({ payload }, { put }) {
        yield put({ type: "setParams", payload });
        yield put({ type: "paging", payload: {} });
      },
    };
  }
};

export const defaultFormBuild = (props) => {

  return [
    defaultHandler.finish(props),
    defaultHandler.submit(props)
  ]
}

export const defaultHandler = {
  submit: ({ form, result, okCallback }) => {
    okCallback(() => {
      form.submit();
      return result || false;
    });
  },
  finish: ({ dispatch, type, closeCallback }) => {
    return (data) => {
      dispatch({ type, payload: { data, callback: closeCallback } });
    }
  }
};

export const defaultOperation = {
  add: (props) => {
    const { dispatch, title, element, icon } = props;
    FormModal.confirm({
      title: title || "Add",
      destroyOnClose: true,
      icon: icon || (<></>),
      closable: false,
      contentElement: element,
      contentProps: { ...props }
    });
  },
  edit: (props) => {
    const { dispatch, type, title, data, element, icon } = props;
    dispatch({
      type: type,
      payload: {
        data: data,
        callback: (model) => {
          FormModal.confirm({
            title: title || "Edit",
            destroyOnClose: true,
            icon: icon || (<></>),
            closable: false,
            contentElement: element,
            contentProps: {
              ...props,
              model
            }
          });
        }
      }
    });
  }
};