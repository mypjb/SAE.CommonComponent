import request from "../projectConfigRelevanceService"
import { defaultModel, parsingPayload } from "@/utils/utils";
export default {
  state: {
    ...defaultModel.state
  },
  reducers: {
    ...defaultModel.reducers
  },
  effects: {
    ...defaultModel.effects({ request, name: "projectConfigRelevance" }),
    *relevance({ payload }, { call, put }) {
      console.log(payload);
      const { callback, data } = parsingPayload(payload);
      yield call(request.relevance, data);
      yield put({ type: "projectConfig/paging" });
      if (callback) {
        callback();
      }
    }
  }
};