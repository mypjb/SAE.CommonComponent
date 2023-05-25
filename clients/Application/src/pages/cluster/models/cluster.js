import request from "../service";
import { history } from "umi";
import { defaultModel } from "@/utils/utils"

export default {
  namespace: "cluster",
  state: {
    ...defaultModel.state
  },
  reducers: {
    ...defaultModel.reducers
  },
  effects: {
    ...defaultModel.effects({ request, name: "cluster" })
  }
};