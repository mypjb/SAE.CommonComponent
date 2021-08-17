import request from "../service";
import { history } from "umi";
import { defaultModel } from "@/utils/utils"

export default {
  state: {
    ...defaultModel.state
  },
  reducers: {
    ...defaultModel.reducers
  },
  effects: {
    ...defaultModel.effects({ request, name: "cluster" })
  },
  subscriptions: {
    setup({ dispatch, history }) {
      history.listen(({ pathname }) => {
        if (pathname === '/cluster') {
          dispatch({
            type: 'paging'
          });
        }
      });
    },
  }
};