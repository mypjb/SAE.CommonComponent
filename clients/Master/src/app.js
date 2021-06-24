import { useState } from 'react';
import { history } from 'umi';
import apps from '../config/app'
import indexPage from './pages/index';

const callBackUrlKey = "saeCallbackUrl";


const appProps = {
    "siteConfig": {
        "appId": "localhost.dev",
        "appName": "master.dev",
        "authority": "http://oauth.sae.com",
        "redirectUris": "http://localhost:8000/oauth/signin-oidc",
        "postLogoutRedirectUris": "http://localhost:8000/oauth/signout-oidc",
        "signIn": "/oauth",
        "login": "http://oauth.sae.com/account/login",
        "apiHost": "http://api.sae.com",
        "callbackUrl": function () {
            const url = window.sessionStorage.getItem(callBackUrlKey);
            window.sessionStorage.removeItem(callBackUrlKey);
            return url || "/routing";
        }
    },
    "api": {
        "menu": "http://api.sae.com/menu/tree"
    }
};


const hideLayoutUrls = ['/identity', '/oauth'];

const processingData = function (menus) {
    const list = [];
    for (let index = 0; index < menus.length; index++) {
        const element = menus[index];
        let data = {
            ...element,
            hideInMenu: element.hidden,
            component: indexPage,
        };
        if (hideLayoutUrls.findIndex(s => (s.indexOf(element.path) != -1)) != -1) {
            data.headerRender = false;
            data.menuRender = false;
            data.menuHeaderRender = false;
        }
        data.routes = processingData(element.items);
        list.push(data);
    }
    console.log({list});
    return list;
}


export const qiankun = fetch(appProps.api.menu).then(async (response) => {
    const routes = processingData(await response.json());
    return {
        // 注册子应用信息
        apps,
        routes,
        layout: {
            name: 'SAE',
            locale: true,
            layout: 'side'
        },
        lifeCycles: {
            afterMount: props => {
                console.log(props);
            },
        },
        addGlobalUncaughtErrorHandler: e => console.log(e)
    }
});


const checkLogin = (user) => {
    if (user) {
        const time = new Date().getTime() / 1000;
        if (user.expires_at && user.expires_at > time) {
            return true;
        }
    }
    return false;
}
export function useQiankunStateForSlave() {

    const [masterState, setMasterState] = useState(appProps);
    const initial = (requestConfig) => {
        requestConfig.prefix = appProps.siteConfig.apiHost;
        requestConfig.credentials = "include";
        requestConfig.middlewares = [async (ctx, next) => {
            const req = ctx.req;
            const options = req.options;

            if (!checkLogin(masterState?.user)) {
                window.sessionStorage.setItem(callBackUrlKey, window.location.pathname + window.location.search);
                history.push(masterState.siteConfig.signIn);
                return;
            }
            const token = masterState?.user?.access_token;

            req.options = {
                ...options,
                headers: {
                    Authorization: "Bearer " + token
                }
            }
            await next();
        }];
        // requestConfig.requestInterceptors = [(url, options) => {

        //     if (!checkLogin(masterState?.user)) {
        //         history.push(masterState.siteConfig.signIn);
        //         //window.location.href = masterState.siteConfig.signIn;
        //         return;
        //     }
        //     const token = masterState?.user?.access_token;
        //     return {
        //         url,
        //         options: {
        //             ...options,
        //             headers: {
        //                 Authorization: "Bearer " + token
        //             }
        //         },
        //     };
        // }];
        requestConfig.errorConfig = {
            adaptor: function (resData, context) {
                if (resData === context.res) {
                    return {
                        ...resData,
                        success: true
                    }
                }
                return {
                    ...resData,
                    success: false,
                    errorMessage: resData.message || resData.title || resData.statusText,
                };
            }
        };
    };

    return {
        initial,
        masterState,
        setMasterState,
        masterPush: (url) => {
            history.push(url);
        }
    };
}

export const layout = () => {
    return {
        name: 'SAE',
        locale: true,
        layout: 'side'
    };
};

