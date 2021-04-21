import request from "../service";
import { defaultModel } from "@/utils/utils"

export default {
  state: {
    ...defaultModel.state
  },
  reducers: {
    ...defaultModel.reducers
  },
  effects: {
    ...defaultModel.effects({ request, name: "template", addSuccessUrl: '/template' })
  },
  subscriptions: {
    setup({ dispatch, history }) {
      history.listen(({ pathname }) => {
        if (pathname === '/template') {
          dispatch({
            type: 'paging',
          });
        }
      });
    },
  }
};