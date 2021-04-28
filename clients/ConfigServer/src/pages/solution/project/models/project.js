import request from "../service";
import { defaultModel } from '@/utils/utils';
import { useParams, useRouteMatch } from 'umi';

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
        const { pathname } = props;
        const pathName = '/solution/project';
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