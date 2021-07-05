const ENV = process.env.UMI_ENV;

let apps = [
    {
        name: 'config-server', // 唯一 id
        entry: '//localhost:8001', // html entry
        path: "/config",
    },
    {
        name: 'identity', // 唯一 id
        entry: '//localhost:8002', // html entry
        path: "/identity",

    },
    {
        name: 'oauth', // 唯一 id
        entry: '//localhost:8003', // html entry
        path: "/oauth",

    },
    {
        name: 'routing', // 唯一 id
        entry: '//localhost:8004', // html entry
        path: "/routing",
    },
    {
        name: 'authorize', // 唯一 id
        entry: '//localhost:8005', // html entry
        path: "/auth",
    }
];


const callBackUrlKey = "saeCallbackUrl";


let appConfig = {
    callBackUrlKey,
    apps,
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

if (ENV == "prod") {
    apps[0].entry = '//configserver.client.sae.com';
    apps[1].entry = '//identity.client.sae.com';
    apps[2].entry = '//oauth.client.sae.com';
    apps[3].entry = '//routing.client.sae.com';
    apps[4].entry = '//authorize.client.sae.com';
    apps.push({
        name: 'user', // 唯一 id
        entry: '//localhost:8000', // html entry
        path: "/user",
    });
    appConfig.siteConfig = {
        ...appConfig.siteConfig,
        "appId": "localhost.test",
        "appSecret": "localhost.test",
        "appName": "master.test",
        "redirectUris": "http://master.client.sae.com/oauth/signin-oidc",
        "postLogoutRedirectUris": "http://master.client.sae.com/oauth/signout-oidc"
    }
}
console.log({ ENV, appConfig });
export default appConfig;
