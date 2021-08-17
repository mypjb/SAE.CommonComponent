import request from "../service";
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
    ...defaultModel.effects({ request, name: "role" })
  },
  subscriptions: {
    setup({ dispatch, history }) {
      history.listen(({ pathname }) => {
        if (pathname === '/role') {
          dispatch({
            type: 'paging',
          });
        }
      });
    },
  }
};