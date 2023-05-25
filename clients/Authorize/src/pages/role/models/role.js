import request from "../service";
import { defaultModel, parsingPayload } from '@/utils/utils'

export default {
  namespace: "role",
  state: {
    ...defaultModel.state,
    tree: []
  },
  reducers: {
    ...defaultModel.reducers
  },
  effects: {
    ...defaultModel.effects({ request, name: "role" })
  }
};