import { RequestConfig, useModel } from 'umi';

const ENV = process.env.NODE_ENV;


let apps = [];
const appProps={
    "siteConfig": {
        "appId": "localhost.dev",
        "appName": "master.dev",
        "authority": "http://api.sae.com",
        "redirectUris": "http://localhost:8000/oauth/signin-oidc" ,
        "postLogoutRedirectUris": "http://localhost:8000/oauth/signout-oidc",
        "signIn": "http://localhost:8000/identity/login"
      }    
};
if (ENV == "development") {
    apps = [
        {
            name: 'config-server', // 唯一 id
            entry: '//dev.sae.com:8001', // html entry
            props:appProps
        },
        {
            name: 'identity', // 唯一 id
            entry: '//dev.sae.com:8002', // html entry
            props:appProps
        },
        {
            name: 'oauth', // 唯一 id
            entry: '//dev.sae.com:8003', // html entry
            props:appProps
        },
        {
            name: 'routing', // 唯一 id
            entry: '//dev.sae.com:8004', // html entry
            props:appProps
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

const menuData = [
    {
        path: '/oauth',
        name: 'Login',
        icon: 'icon-smile'
    },
    {
        name: "config",
        children: [
            {
                path: "/config-server/solution",
                name: "solution"
            },
            {
                path: "/config-server/template",
                name: "template"
            }
        ]
    },
    {
        path: "/routing/menu",
        name: "menu"
    }
];

export const qiankun = function () {
    return {
        // 注册子应用信息
        apps:apps,
        lifeCycles: {
            afterMount: props => {
                console.log(props);
            },
        },
        addGlobalUncaughtErrorHandler: e => console.log(e)
    }
};

export const layout = {
    name: "SAE",
    menuDataRender: () => {
        return menuData;
    }
};

export async function getInitialState() { 
    return {
        apps
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
