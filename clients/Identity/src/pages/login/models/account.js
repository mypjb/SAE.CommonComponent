import request from "../service";
import { history } from 'umi';

export default {
    state: {
        account: {
            name: "",
            password: "",
            Remember: true
        }
    },
    reducers: {
        clear(state) {
            return {
                ...state, account: {
                    name: "",
                    password: "",
                    Remember: true
                }
            };
        }
    },
    effects: {
        *login({ payload }, { call, put }) {
            const data = yield call(request.login, payload);
            //yield put({ type: "clear" });
            location.href = data.returnUrl;
        }
    },
    subscriptions: {
        setup({ dispatch, history }) {
            history.listen(({ pathname }) => {
                if (pathname === '/account/login') {
                    dispatch({
                        type: 'clear',
                    });
                }
            });
        },
    }
};