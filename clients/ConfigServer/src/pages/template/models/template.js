import request from "../service";
import { defaultModel } from "@/utils/utils"
import { useParams, useRouteMatch } from "umi";

export default {
  state: {
    ...defaultModel.state
  },
  reducers: {
    ...defaultModel.reducers
  },
  effects: {
    ...defaultModel.effects({ request, name: "template" }),
    *list(payload, { call }) {
      const { callback } = payload;
      const data = yield call(request.list);
      if (callback) {
        callback(data);
      }
    }
  },
  subscriptions: {
    setup({ dispatch, history }) {
      history.listen(({ pathname }) => {
        if (pathname === '/template') {
          dispatch({
            type: 'paging'
          });
        }
      });
    },
  }
};