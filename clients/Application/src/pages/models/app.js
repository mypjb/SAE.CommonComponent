import request from "../service";
import { defaultModel, parsingPayload } from '@/utils/utils'

export default {
  state: {
    ...defaultModel.state
  },
  reducers: {
    ...defaultModel.reducers
  },
  effects: {
    ...defaultModel.effects({ request, name: "app" }),
    *refreshSecret({ payload }, { call, put }) {

      const { callback, data } = parsingPayload(payload);

      const stream = yield call(request.refreshSecret, data);
      const blob = new Blob([stream]);
      const objectURL = URL.createObjectURL(blob);
      let btn = document.createElement('a');
      btn.download = 'appSecret.txt';
      btn.href = objectURL;
      btn.click();
      URL.revokeObjectURL(objectURL);
      btn = null;
      callback();
    }
  },
  subscriptions: {
    setup({ dispatch, history }) {
      history.listen(({ pathname }) => {
        if (pathname === '/') {
          dispatch({
            type: 'paging',
          });
        }
      });
    },
  }
};