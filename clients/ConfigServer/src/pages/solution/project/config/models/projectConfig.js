import { defaultModel } from '@/utils/utils';
import request from "../service"

export default {
  state: {
    ...defaultModel.state
  },
  reducers: {
    ...defaultModel.reducers
  },
  effects: {
    ...defaultModel.effects({ request, name: "projectConfig" }),
    *refresh(_, { put, select }) {
      const paging = yield select(({ projectConfig }) => (projectConfig.paging));
      yield put({ type: "paging", payload: paging });
    },
    *relevance({ payload }, { put }) {
      yield put({ type: "relevance/search", payload });
    }
  }
};