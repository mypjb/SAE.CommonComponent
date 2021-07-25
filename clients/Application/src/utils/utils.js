import FormModal from "@/components/FormModal";
import { Modal, Switch } from "antd";

export const Format = {
  status: (status, props) => {
    return (<Switch checkedChildren="Enable" unCheckedChildren="Disable" checked={status == 1} {...props} />);
  },
  date: (date, props) => {
    return new Date(+new Date(new Date(date).toJSON()) + 8 * 3600 * 1000).toISOString().replace(/T/g, ' ').replace(/\.[\d]{3}Z/, '');
  }
}

//default state
export const defaultState = {
  paging: {
    pageIndex: 1,
    pageSize: 10,
    totalCount: 0
  },
  items: [],
  params: {},
};

//parsing payload
export const parsingPayload = (payload) => {
  let model = {};
  if (payload && payload.callback) {
    model.callback = payload.callback;
  } else {
    model.callback = () => { };
  }
  if (payload && payload.data) {
    model.data = payload.data;
  } else {
    model.data = payload;
  }
  return model;
};


//default dispatch type
export const defaultDispatchType = (name) => {
  return {
    add: `${name}/add`,
    delete: `${name}/delete`,
    edit: `${name}/edit`,
    find: `${name}/find`,
    search: `${name}/search`,
    paging: `${name}/paging`,
  }
}

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
      return { ...state, params: { ...state.params, ...payload } };
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

        callback();

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
        callback();
      },
      *status({ payload }, { call, put }) {
        const { callback, data } = parsingPayload(payload);
        yield call(request.status, data);
        yield put({ type: "paging", payload: {} });
        callback();
      },
      *find({ payload }, { put, call }) {
        const { callback, data } = parsingPayload(payload);
        const model = yield call(request.find, data);
        callback(model);
      },
      *paging({ payload }, { call, put, select }) {

        const { callback, data } = parsingPayload(payload);
        const params = yield select((globalStatus) => (globalStatus[stateName].params));

        const paging = yield call(request.queryPaging, { ...data, ...params });

        yield put({ type: "setList", payload: paging });

        yield put({ type: "setPaging", payload: paging });

        const state = yield select((globalStatus) => (globalStatus[stateName]));

        callback(state);
      },
      *search({ payload }, { put }) {
        yield put({ type: "setParams", payload });
        yield put({ type: "paging", payload: {} });
      },
    };
  }
};



//default Form Build fn
export const defaultFormBuild = (props) => {
  return [
    defaultHandler.finish(props),
    defaultHandler.submit(props)
  ]
}

//default handler fn
export const defaultHandler = {
  submit: ({ form, result, okCallback }) => {
    okCallback(() => {
      form.submit();
      return result || false;
    });
  },
  finish: ({ dispatch, dispatchType, closeCallback }) => {
    return (data) => {
      dispatch({ type: dispatchType, payload: { data, callback: closeCallback } });
    }
  },
  delete: ({ dispatch, dispatchType }) => {
    return (data) => {
      Modal.confirm({
        title: 'Are you sure delete this task?',
        onOk: () => {
          dispatch({
            type: dispatchType,
            payload: data,
          });
        }
      });
    }
  },
  search: ({ dispatch, dispatchType }) => {
    return (payload) => {
      dispatch({
        type: dispatchType,
        payload: {
          ...payload
        }
      });
    }
  }
};

//default operation
export const defaultOperation = {
  add: (props, proxyModal) => {
    const { dispatch, title, element, icon, modalProps } = props;
    FormModal.confirm({
      title: "Add",
      destroyOnClose: true,
      icon: icon || (<></>),
      closable: false,
      ...(modalProps || {}),
      contentElement: element,
      contentProps: { ...props }
    }, proxyModal);
  },
  edit: (props, proxyModal) => {
    const { dispatch, type, title, data, element, icon, modalProps } = props;
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
            ...(modalProps || {}),
            contentElement: element,
            contentProps: { ...props, model }
          }, proxyModal);
        }
      }
    });
  }
};
