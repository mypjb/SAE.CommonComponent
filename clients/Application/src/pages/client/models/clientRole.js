import request from "../clientRoleService";
import { defaultModel, parsingPayload } from '@/utils/utils'

export default {
  namespace: "clientRole",
  state: {
    ...defaultModel.state,
    tree: []
  },
  reducers: {
    ...defaultModel.reducers,
  },
  effects: {
    ...defaultModel.effects({ request, name: "clientRole" }),
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