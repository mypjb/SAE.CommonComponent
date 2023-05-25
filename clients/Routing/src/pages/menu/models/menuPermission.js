import request from "../menuPermissionService";
import { defaultModel } from '@/utils/utils'

export default {
  namespace: "menuPermission",
  state: {
    ...defaultModel.state
  },
  reducers: {
    ...defaultModel.reducers
  },
  effects: {
    ...defaultModel.effects({ request, name: "menuPermission" }),
  }
};