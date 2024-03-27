import request from "../service";
import { defaultModel } from '@/utils/utils'

export default {
  namespace: "rule",
  state: {
    ...defaultModel.state
  },
  reducers: {
    ...defaultModel.reducers
  },
  effects: {
    ...defaultModel.effects({ request, name: "rule" })
  }
};