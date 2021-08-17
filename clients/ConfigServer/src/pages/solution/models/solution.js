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
    ...defaultModel.effects({ request, name: "solution" })
  },
  subscriptions: {
    setup({ dispatch, history }) {
      history.listen(({ pathname }) => {
        if (pathname === '/solution') {
          dispatch({
            type: 'paging'
          });
        }
      });
    },
  }
};