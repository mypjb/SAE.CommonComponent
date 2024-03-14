import Cluster from '@/components/Cluster';
import { Space } from 'antd';
import { history, useModel } from 'umi';
import { load, userManager } from '../config/appConfig';

let globalConfig = {};

const hideLayoutUrls = ['/identity', '/oauth'];

export const request: RequestConfig = {};

const processingMenuData = function (menus, apps) {
  const list = [];
  for (let index = 0; index < menus.length; index++) {
    const element = menus[index];
    let data = {
      ...element,
      hideInMenu: element.hidden,
      hideInBreadcrumb: false,
    };

    const routPath = element.path.toLowerCase();

    let appIndex = apps.findIndex((s) => {
      const appPath = s.path.toLowerCase();
      if (appPath == routPath) {
        return true;
      }
      return false;
    });

    if (appIndex == -1) {
      appIndex = apps.findIndex((s) => {
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

    if (hideLayoutUrls.findIndex((s) => s.indexOf(element.path) != -1) != -1) {
      data.headerRender = false;
      data.menuRender = false;
      data.menuHeaderRender = false;
    }
    data.routes = processingMenuData(element.items, apps);
    list.push(data);
  }
  return list;
};

const processingAppData = function (apps) {
  const array = [];

  for (let index = 0; index < apps.length; index++) {
    const element = apps[index];
    if (element.entry && element.path) {
      array.push({ ...element, autoCaptureError: true });
    }
  }

  return array;
};

export const qiankun = async function () {
  globalConfig = await load();

  const { siteConfig } = globalConfig;

  const { api } = siteConfig;

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
      afterMount: (props) => console.log(props),
    },
    addGlobalUncaughtErrorHandler: (e) => console.log(e),
  };
};

const checkLogin = (user) => {
  if (user) {
    const time = new Date().getTime() / 1000;
    if (user.expires_at && user.expires_at > time) {
      return true;
    }
  }
  return false;
};

const replaceRequestParams = function (key, val, ops) {
  if (ops.method.toLowerCase() == 'get') {
    if (ops?.params && !ops.params[key]) {
      ops.params[key] = val;
    }
  } else {
    if (ops?.data && !ops.data[key]) {
      ops.data[key] = val;
    }
  }
  console.log({ key, val, ops });
};

const configureRequest = (requestConfig, slaveConfig) => {
  slaveConfig = slaveConfig || {
    api: {
      globalCluster: 'clusterId',
      globalApp: 'appId',
    },
  };
  const initialState = globalConfig;
  requestConfig.baseURL = initialState.siteConfig.api.host;
  // requestConfig.credentials = "include";
  requestConfig.requestInterceptors = [
    (ops) => {
      const { url } = initialState.siteConfig;

      const { headers, method } = ops;

      if (!checkLogin(initialState?.user)) {
        window.sessionStorage.setItem(
          initialState.callBackUrlKey,
          window.location.pathname + window.location.search,
        );
        if (url.signIn.startsWith('http') || url.signIn.startsWith('//')) {
          window.location.href = url.oauth;
        } else {
          history.push(url.oauth);
        }
        return;
      }
      const token = initialState?.user?.access_token;

      headers[method]['Authorization'] = 'Bearer ' + token;

      console.log({ ops, globalConfig });

      if (slaveConfig?.api?.globalApp && initialState?.globalData?.appId) {
        replaceRequestParams(
          slaveConfig?.api?.globalApp,
          initialState.globalData.appId,
          ops,
        );
      } else {
        if (
          slaveConfig?.api?.globalCluster &&
          initialState?.globalData?.cluserId
        ) {
          replaceRequestParams(
            slaveConfig?.api?.globalCluster,
            initialState.globalData.cluserId,
            ops,
          );
        }
      }

      return { ...ops };
    },
  ];

  requestConfig.responseInterceptors = [];

  requestConfig.errorConfig = {
    errorHandler: (res) => {
      console.log(res);
      const { status, statusText } = res.response;

      if (status != '200') {
        const error = new Error(statusText);
        throw error; // 抛出自制的错误
      }
    },
    errorThrower: (error, opt) => {
      console.log({ error, opt });
    },
  };
};

export function useQiankunStateForSlave() {
  const { initialState, setInitialState } = useModel('@@initialState');
  return {
    initial: configureRequest,
    masterState: initialState || globalConfig,
    setMasterState: (data) => {
      const configData = { ...globalConfig, ...data };
      globalConfig = configData;
      setInitialState(configData);
    },
    masterPush: (url) => {
      history.push(url);
    },
  };
}

export const layout = ({ initialState }) => {
  const layoutOptions = {
    name: initialState.siteConfig.basicInfo.name,
    locale: true,
    layout: 'side',
  };

  if (checkLogin(initialState?.user)) {
    configureRequest(request);
    layoutOptions.logout = function () {
      history.push(initialState.siteConfig.url.logout || '/oauth/logout');
    };

    const selectHandler = () => {
      window.location.reload();
    };

    layoutOptions.menuHeaderRender = (logo, title) => {
      return (
        <div>
          <div>
            <a href="/">
              <Space>
                {logo}
                {title}
              </Space>
            </a>
          </div>
          <div>
            <Cluster selectHandler={selectHandler}></Cluster>
          </div>
        </div>
      );
    };
  }
  return layoutOptions;
};

const formatGlobalConfig = (data) => {
  const userInfo = {
    ...(data?.user?.profile || userManager.get()),
  };
  return {
    ...data,
    userInfo,
  };
};

export const getInitialState = async () => {
  const data = formatGlobalConfig(globalConfig);
  return { ...data };
};
