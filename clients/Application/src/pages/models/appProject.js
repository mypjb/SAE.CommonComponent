import request from "../appProjectService";
import { defaultModel, parsingPayload } from '@/utils/utils'

export default {
  state: {
    ...defaultModel.state,
    tree: []
  },
  reducers: {
    ...defaultModel.reducers,
  },
  effects: {
    ...defaultModel.effects({ request, name: "appProject" }),
    *add({ payload }, { call, put }) {

      const { callback, data } = parsingPayload(payload);

      yield call(request.add, data);

      callback();
    },
    *delete({ payload }, { call, put }) {
      const { callback, data } = parsingPayload(payload);
      yield call(request.delete, data);
      callback();
    }
  }
};