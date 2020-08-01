import request from "../service";

export default {
    state: {
        account: {
            name: "",
            password: "",
            IsPersistent: true
        }
    },
    reducers: {
        clear(state) {
            return {
                ...state, account: {
                    name: "",
                    password: "",
                    IsPersistent: true
                }
            };
        }
    },
    effects: {
        *login({ payload }, { call, put }) {
            const data = yield call(request.login, payload);
            yield put({ type: "clear" });
        }
    },
    subscriptions: {
        setup({ dispatch, history }) {
            history.listen(({ pathname }) => {
                if (pathname === '/login') {
                    dispatch({
                        type: 'clear',
                    });
                }
            });
        },
    }
};