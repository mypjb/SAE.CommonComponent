import request from "../service";
import templateRequest from "@/services/template";
import { defaultModel } from "@/utils/utils"

export default {
  state: {
    ...defaultModel.state,
    setTemplateList(state, { payload }) {
      return { ...state, templates: payload }
    }
  },
  reducers: {
    ...defaultModel.reducers
  },
  effects: {
    ...defaultModel.effects({ request, name: "config" }),
    *queryTemplateList(params, { call, put }) {
      const list = yield call(templateRequest.list);
      yield put({ type: 'setTemplateList', payload: list });
    }
  },
  subscriptions: {
    setup({ dispatch, history }) {
      history.listen((props) => {
        const { pathname } = props;
        const pathName = '/solution/config';
        const index = pathname.toLocaleLowerCase().indexOf(pathName);
        if (index === 0) {
          const solutionId = pathname.substr(pathName.length + 1);
          dispatch({
            type: 'search',
            payload: {
              solutionId
            }
          });
        }
      });
    },
  }
};
