import request from "../service";
import { defaultModel } from '@/utils/utils';
export default {
  state: {
    ...defaultModel.state
  },
  reducers: {
    ...defaultModel.reducers
  },
  effects: {
    ...defaultModel.effects({ request, name: "project" })
  },
  subscriptions: {
    setup({ dispatch, history }) {
      history.listen((props) => {
        console.log({ history, props });
        const { pathname } = props;
        if (pathname.toLocaleLowerCase().indexOf('/solution/project') === 0) {
          dispatch({
            type: 'paging',
          });
        }
      });
    },
  }
};