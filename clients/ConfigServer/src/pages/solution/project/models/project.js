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
  }
};