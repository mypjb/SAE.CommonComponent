import request from "../projectConfigReferenceService"
import { defaultModel, parsingPayload } from "@/utils/utils";
export default {
  state: {
    ...defaultModel.state
  },
  reducers: {
    ...defaultModel.reducers
  },
  effects: {
    ...defaultModel.effects({ request, name: "projectConfigReference" }),
    *reference({ payload }, { call, put }) {
      console.log(payload);
      const { callback, data } = parsingPayload(payload);
      yield call(request.reference, data);
      yield put({ type: "projectConfig/paging" });
      if (callback) {
        callback();
      }
    }
  }
};
