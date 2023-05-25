import request from "../appConfigReferenceService"
import { defaultModel, parsingPayload } from "@/utils/utils";
export default {
  namespace: "appConfigReference",
  state: {
    ...defaultModel.state
  },
  reducers: {
    ...defaultModel.reducers
  },
  effects: {
    ...defaultModel.effects({ request, name: "appConfigReference" }),
    *reference({ payload }, { call, put }) {
      console.log(payload);
      const { callback, data } = parsingPayload(payload);
      yield call(request.reference, data);
      yield put({ type: "appConfig/paging" });
      if (callback) {
        callback();
      }
    }
  }
};
