import request from "../service";
import { defaultModel } from "@/utils/utils"
import { useModel } from 'umi';

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
    ...defaultModel.effects({ request, name: "config" })
  },
  subscriptions: {
    setup({ dispatch, history }) {
      history.listen((props) => {
        // const { pathname } = props;
        // const pathName = '/solution/config';
        // const index = pathname.toLocaleLowerCase().indexOf(pathName);
        // if (index === 0) {
        //   const solutionId = pathname.substr(pathName.length + 1);
        //   dispatch({
        //     type: 'search',
        //     payload: {
        //       solutionId
        //     }
        //   });
        // }
      });
    },
  }
};
