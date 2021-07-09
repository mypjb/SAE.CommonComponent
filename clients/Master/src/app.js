import { useState } from 'react';
import { history } from 'umi';
import appConfig from '../config/appConfig'
import indexPage from './pages/index';



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
    console.log({ list });
    return list;
}

export const qiankun = fetch(appConfig.api.app).then(async (response) => {
    console.log("qiankun");
    const apps = await response.json();
    const menuResponse = await fetch(appConfig.api.menu);
    const routes = processingData(await menuResponse.json());
    appConfig.apps = apps;
    appConfig.routes = routes;
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
    const [masterState, setMasterState] = useState(appConfig);
    const initial = (requestConfig) => {
        requestConfig.prefix = appConfig.siteConfig.apiHost;
        requestConfig.credentials = "include";
        requestConfig.middlewares = [async (ctx, next) => {
            const req = ctx.req;
            const options = req.options;

            if (!checkLogin(masterState?.user)) {
                window.sessionStorage.setItem(appConfig.callBackUrlKey, window.location.pathname + window.location.search);
                if (masterState.siteConfig.signIn.startsWith('http')) {
                    window.location.href = masterState.siteConfig.signIn;
                } else {
                    history.push(masterState.siteConfig.signIn);
                }
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

export const getInitialState = () => {
    return appConfig;
}

