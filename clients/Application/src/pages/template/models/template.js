import request from "../service";
import { defaultModel } from "@/utils/utils"

export default {
  namespace: "template",
  state: {
    ...defaultModel.state
  },
  reducers: {
    ...defaultModel.reducers
  },
  effects: {
    ...defaultModel.effects({ request, name: "template" }),
    *list(payload, { call }) {
      const { callback } = payload;
      const data = yield call(request.list);
      if (callback) {
        callback(data);
      }
    }
  }
};