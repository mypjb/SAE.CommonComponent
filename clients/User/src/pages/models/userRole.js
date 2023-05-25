import request from "../userRoleService";
import { defaultModel, parsingPayload } from '@/utils/utils'

export default {
  namespace: "userRole",
  state: {
    ...defaultModel.state,
    tree: []
  },
  reducers: {
    ...defaultModel.reducers,
  },
  effects: {
    ...defaultModel.effects({ request, name: "userRole" }),
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