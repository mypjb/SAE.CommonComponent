import { useState } from 'react';
import { history } from 'umi';
import { load } from '../config/appConfig'
import indexPage from './pages/index';

let globalConfig = {};

const hideLayoutUrls = ['/identity', '/oauth'];

const processingMenuData = function (menus) {
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
        data.routes = processingMenuData(element.items);
        list.push(data);
    }
    console.log({ list });
    return list;
}

const processingAppData = function (apps) {

    const array = [];

    for (let index = 0; index < apps.length; index++) {
        const element = apps[index];
        if (element.entry && element.path) {
            array.push(element);
        }
    }

    return array;
}

export const qiankun = async function () {

    globalConfig = await load();

    const { api } = globalConfig.siteConfig;

    const apps = processingAppData(await (await fetch(api.app)).json());

    const menus = await (await fetch(api.menu)).json();

    const routes = processingMenuData(menus);

    globalConfig.apps = apps;

    globalConfig.routes = routes;

    console.log(globalConfig);

    return {
        // 注册子应用信息
        apps,
        routes,
        // layout: {
        //     name: globalConfig.siteConfig.appName,
        //     locale: true,
        //     layout: 'side'
        // },
        lifeCycles: {
            afterMount: props => {
                console.log(props);
            },
        },
        addGlobalUncaughtErrorHandler: e => console.log(e)
    }

};


// fetch(appConfig.api.app).then(async (response) => {

// });

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
    const [masterState, setMasterState] = useState(globalConfig);
    const initial = (requestConfig) => {
        requestConfig.prefix = globalConfig.siteConfig.api.host;
        requestConfig.credentials = "include";
        requestConfig.middlewares = [async (ctx, next) => {

            const { url } = masterState.siteConfig;
            const req = ctx.req;
            const options = req.options;

            if (!checkLogin(masterState?.user)) {
                window.sessionStorage.setItem(globalConfig.callBackUrlKey, window.location.pathname + window.location.search);
                if (url.signIn.startsWith('http')) {
                    window.location.href = url.oauth;
                } else {
                    history.push(url.oauth);
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

    const layoutOptions = {
        name: globalConfig.siteConfig.basicInfo.name,
        locale: true,
        layout: 'side',

    };

    if (globalConfig?.user) {
        layoutOptions.logout = function () {
            history.push(globalConfig.siteConfig.url.logout || "/oauth/logout");
        }
    }
    return layoutOptions;
};

export const getInitialState = async () => {
    const userInfo = {
        ...(globalConfig?.user?.profile || {})
    };
    return {
        ...globalConfig,
        ...userInfo
    };
}

