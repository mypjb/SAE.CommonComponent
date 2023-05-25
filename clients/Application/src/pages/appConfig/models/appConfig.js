import { defaultModel } from '@/utils/utils';
import request from "../service"

export default {
  namespace: "appConfig",
  state: {
    ...defaultModel.state
  },
  reducers: {
    ...defaultModel.reducers
  },
  effects: {
    ...defaultModel.effects({ request, name: "appConfig" }),
    *refresh(_, { put, select }) {
      const paging = yield select(({ appConfig }) => (appConfig.paging));
      yield put({ type: "paging", payload: paging });
    },
    *reference({ payload }, { put }) {
      yield put({ type: "reference/search", payload });
    }
  }
};
