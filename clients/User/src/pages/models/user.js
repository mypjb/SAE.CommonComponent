import request from "../service";
import { defaultModel, parsingPayload } from '@/utils/utils'

export default {
  namespace: "user",
  state: {
    ...defaultModel.state
  },
  reducers: {
    ...defaultModel.reducers
  },
  effects: {
    ...defaultModel.effects({ request, name: "user" })
  }
};