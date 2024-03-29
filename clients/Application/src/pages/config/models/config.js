import request from "../service";
import { defaultModel, regex } from "@/utils/utils";

export default {
  state: {
    ...defaultModel.state,
    setTemplateList(state, { payload }) {
      return { ...state, templates: payload }
    }
  },
  reducers: {
    ...defaultModel.reducers
  },
  effects: {
    ...defaultModel.effects({ request, name: "config" })
  }
};
