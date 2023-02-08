import { useState } from 'react';
import { history, useModel } from 'umi';
import { load, userManager } from '../config/appConfig'
import indexPage from './pages/index';

let globalConfig = {};

const hideLayoutUrls = ['/identity', '/oauth'];

const processingMenuData = function (menus, apps) {

    const list = [];
    for (let index = 0; index < menus.length; index++) {
        const element = menus[index];
        let data = {
            ...element,
            hideInMenu: element.hidden,
            hideInBreadcrumb: false
        };

        const routPath = element.path.toLowerCase();

        let appIndex = apps.findIndex(s => {
            const appPath = s.path.toLowerCase();
            if (appPath == routPath) {
                return true;
            }
            return false;
        });

        if (appIndex == -1) {
            appIndex = apps.findIndex(s => {
                const appPath = s.path.toLowerCase();
                if (appPath.startsWith(routPath)) {
                    return true;
                }
                return false;
            });
        }

        if (appIndex != -1) {
            data.microApp = apps[appIndex].name;
        }

        if (hideLayoutUrls.findIndex(s => (s.indexOf(element.path) != -1)) != -1) {
            data.headerRender = false;
            data.menuRender = false;
            data.menuHeaderRender = false;
        }
        data.routes = processingMenuData(element.items, apps);
        list.push(data);
    }
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

    const routes = processingMenuData(menus, apps);

    globalConfig.apps = apps;

    globalConfig.routes = routes;

    return {
        // 注册子应用信息
        apps,
        routes,
        lifeCycles: {
            afterMount: props => {
            },
        },
        addGlobalUncaughtErrorHandler: e => console.log(e)
    }

};


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

    const { setInitialState } = useModel('@@initialState');
    const [masterState, setMasterState] = useState(globalConfig);
    const initial = (requestConfig) => {
        requestConfig.prefix = globalConfig.siteConfig.api.host;
        requestConfig.credentials = "include";
        requestConfig.requestInterceptors = [(requestPath, ops) => {

            const { url } = masterState.siteConfig;

            const { headers, method } = ops;

            if (!checkLogin(masterState?.user)) {
                window.sessionStorage.setItem(globalConfig.callBackUrlKey, window.location.pathname + window.location.search);
                if (url.signIn.startsWith('http') || url.signIn.startsWith('//')) {
                    window.location.href = url.oauth;
                } else {
                    history.push(url.oauth);
                }
                return;
            }
            const token = masterState?.user?.access_token;

            headers[method]["Authorization"] = "Bearer " + token;

            debugger;

            return { requestPath, ops };
        }];

        requestConfig.responseInterceptors = [];

        requestConfig.errorConfig = {
            errorHandler: (res) => {
                console.log(res)
                debugger;
            },
            errorThrower: (error, opts) => {
                console.log({error, opts})
                debugger;
            }
        };
    };

    return {
        initial,
        masterState,
        setMasterState: (data) => {
            setMasterState(data);
            const initialData = formatGlobalConfig(data);
            setInitialState(initialData);
        },
        masterPush: (url) => {
            history.push(url);
        }
    };
}

export const layout = ({ initialState }) => {
    const layoutOptions = {
        name: initialState.siteConfig.basicInfo.name,
        locale: true,
        layout: 'side',
    };

    if (initialState?.user) {
        layoutOptions.logout = function () {
            history.push(initialState.siteConfig.url.logout || "/oauth/logout");
        }
    }
    return layoutOptions;
};



const formatGlobalConfig = (data) => {
    const userInfo = {
        ...(data?.user?.profile || (userManager.get()))
    };
    return {
        ...data,
        ...userInfo
    };
}

export const getInitialState = async () => {
    const data = formatGlobalConfig(globalConfig);
    return {
        ...data
    };
}

