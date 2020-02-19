import {queryPaging} from "@/services/solution"

export default {
    state: {
        pageIndex:1,
        pageSize:10,
        totalCount:0,
        items: [],
        params:{}
    },
    reducers: {
        loadData(state, { payload: { items } }) {
            return { ...state, items};
        },
        setPaging(state, { payload: { pageIndex, pageSize, totalCount } }) {
          return { ...state, pageIndex, pageSize, totalCount };
        },
        setParams(state,{payload}){
          return { ...state, params: { ...payload } };
        }
    },
    effects:{
        *paging({ payload }, { call, put, select}){
            const params = yield select(({ solution }) => (solution.params));
            const data = yield call(queryPaging, { ...payload, ...params });
            yield put({ type: "loadData",payload:data});
            yield put({ type: "setPaging", payload: data });
        },
        *search({ payload }, { call, put }){
          yield put({ type: "setParams", payload });
          yield put({ type: "paging", payload: {} });
        }
    },
    subscriptions: {
        setup({ dispatch, history }) {
          history.listen(({ pathname }) => {
            if (pathname === '/solution') {
              dispatch({
                type: 'solution/paging',
              });
            }
          });
        },
    }
  };