import request from "../service";
import { defaultModel, parsingPayload,regex } from '@/utils/utils'

export default {
  state: {
    ...defaultModel.state
  },
  reducers: {
    ...defaultModel.reducers
  },
  effects: {
    ...defaultModel.effects({ request, name: "client" }),
    *refreshSecret({ payload }, { call, put }) {

      const { callback, data } = parsingPayload(payload);
      debugger;
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
      const idRegex = regex.id('/client/');
      history.listen(({ pathname }) => {
        if (idRegex.test(pathname)) {
          dispatch({
            type: 'search',
            payload:{
              appId:idRegex.exec(pathname)[1]
            }
          });
        }
      });
    },
  }
};