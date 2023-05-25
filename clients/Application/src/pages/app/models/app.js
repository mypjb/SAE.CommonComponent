import request from "../service";
import { defaultModel, parsingPayload, regex } from '@/utils/utils';

export default {
  namespace: "app",
  state: {
    ...defaultModel.state
  },
  reducers: {
    ...defaultModel.reducers
  },
  effects: {
    ...defaultModel.effects({ request, name: "app" }),
    *publish({ payload }, { call, put, select }) {

      const { callback, data } = parsingPayload(payload);

      yield call(request.publish, data);

      callback();

    },
    *preview({ payload }, { call, put, select }) {
      const { callback, data } = parsingPayload(payload);

      const previewData = yield call(request.preview, data);

      callback(previewData);
    }
  }
};