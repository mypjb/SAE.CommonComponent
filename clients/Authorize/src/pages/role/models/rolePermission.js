import request from "../rolePermissionService";
import { defaultModel, parsingPayload } from '@/utils/utils'

export default {
  state: {
    ...defaultModel.state,
    tree: []
  },
  reducers: {
    ...defaultModel.reducers
  },
  effects: {
    ...defaultModel.effects({ request, name: "rolePermission" })
  }
};