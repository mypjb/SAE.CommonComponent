import request from "../service";
import { defaultModel, parsingPayload } from '@/utils/utils';

export default {
  state: {
    ...defaultModel.state
  },
  reducers: {
    ...defaultModel.reducers
  },
  effects: {
    ...defaultModel.effects({ request, name: "project" }),
    *publish({ payload }, { call, put, select }) {

      const { callback, data } = parsingPayload(payload);

      yield call(request.publish, data);

      callback();

    },
    *preview({ payload }, { call, put, select }) {
      const { callback, data } = parsingPayload(payload);

      const model = yield call(request.preview, data);

      callback(model);
    }
  }
};