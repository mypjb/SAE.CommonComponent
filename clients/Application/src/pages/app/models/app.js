import request from "../service";
import { defaultModel, parsingPayload, regex } from '@/utils/utils';

export default {
  state: {
    ...defaultModel.state
  },
  reducers: {
    ...defaultModel.reducers
  },
  effects: {
    ...defaultModel.effects({ request, name: "app" }),
    *publish({ payload }, { call, put, select }) {

      const { callback, data } = parsingPayload(payload);

      yield call(request.publish, data);

      callback();

    },
    *preview({ payload }, { call, put, select }) {
      const { callback, data } = parsingPayload(payload);

      const previewData = yield call(request.preview, data);

      const appData = yield call(request.appConfig, data);

      callback({ preview: previewData, current: appData });
    }
  },
  subscriptions: {
    setup({ dispatch, history }) {
      const idRegex = regex.id('/cluster/app/');

      history.listen(({ pathname }) => {
        if (idRegex.test(pathname)) {
          dispatch({
            type: 'search',
            payload: {
              clusterId: idRegex.exec(pathname)[1]
            }
          });
        }
      });
    },
  }
};