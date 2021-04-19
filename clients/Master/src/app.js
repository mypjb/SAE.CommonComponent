import { useState } from 'react';
import { history } from 'umi';

const ENV = process.env.NODE_ENV;

let apps = [];
const appProps = {
    "siteConfig": {
        "appId": "localhost.dev",
        "appName": "master.dev",
        "authority": "http://oauth.sae.com",
        "redirectUris": "http://localhost:8000/oauth/signin-oidc",
        "postLogoutRedirectUris": "http://localhost:8000/oauth/signout-oidc",
        "signIn": "/oauth",
        "login": "http://oauth.sae.com/account/login",
        "apiHost": "http://api.sae.com"
    }
};
if (ENV == "development") {
    apps = [
        {
            name: 'config-server', // 唯一 id
            entry: '//dev.sae.com:8001', // html entry

        },
        {
            name: 'identity', // 唯一 id
            entry: '//dev.sae.com:8002', // html entry

        },
        {
            name: 'oauth', // 唯一 id
            entry: '//dev.sae.com:8003', // html entry

        },
        {
            name: 'routing', // 唯一 id
            entry: '//dev.sae.com:8004', // html entry

        }
    ];
} else {
    apps = [
        {
            name: 'config-server', // 唯一 id
            entry: '//configserver.client.sae.com', // html entry
        },
        {
            name: 'identity', // 唯一 id
            entry: '//identity.client.sae.com', // html entry
        },
        {
            name: 'oauth', // 唯一 id
            entry: '//oauth.client.sae.com', // html entry
        },
        {
            name: 'routing', // 唯一 id
            entry: '//routing.client.sae.com', // html entry
        }
    ];
}



export const qiankun = function () {
    return {
        // 注册子应用信息
        apps: apps,
        lifeCycles: {
            afterMount: props => {
                console.log(props);
            },
        },
        addGlobalUncaughtErrorHandler: e => console.log(e)
    }
};

export const layout = {
    name: "SAE"
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

    const [masterState, setMasterState] = useState(appProps);
    const initial = (requestConfig) => {
        requestConfig.prefix = appProps.siteConfig.apiHost;
        requestConfig.credentials = "include";
        requestConfig.middlewares=[async (ctx,next)=>{
            const req= ctx.req;
            const options=req.options;
            
            if (!checkLogin(masterState?.user)) {
                history.push(masterState.siteConfig.signIn);
                //window.location.href = masterState.siteConfig.signIn;
                return;
            }
            const token = masterState?.user?.access_token;
            
            req.options={
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

// export const dva = {
//   config: {
//     onError(e) {
//       e.preventDefault();
//       console.error(e.message);
//     },
//   }
// };
