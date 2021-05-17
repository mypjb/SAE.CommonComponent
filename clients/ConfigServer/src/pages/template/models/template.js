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
    ...defaultModel.effects({ request, name: "template" })
  },
  subscriptions: {
    setup({ dispatch, history }) {
      history.listen(({ pathname }) => {
        if (pathname === '/template') {
          dispatch({
            type: 'paging',
            payload: {
              data: {
                pageIndex: 1,
                pageSize: 1,
                totalCount: 0
              }
            },
          });
        }
      });
    },
  }
};