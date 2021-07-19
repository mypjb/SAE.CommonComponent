const ENV = process.env.UMI_ENV;

const callBackUrlKey = "saeCallbackUrl";
const apiHost = "http://api.sae.com";

export const appConfig = {
    callBackUrlKey,
    callbackUrl: function () {
        const url = window.sessionStorage.getItem(callBackUrlKey);
        window.sessionStorage.removeItem(callBackUrlKey);
        return url || "/routing";
    },
    api: {
        menu: apiHost + "/menu/tree",
        app: apiHost + "/plugin/list",
        config: apiHost + "/app/config?appid=localhost.test"
    }
};

if (ENV == "prod") {
    appConfig.api.config += "&env=Production";
} else {
    appConfig.api.config += "&env=Development";
}

export const load = async () => {
    const configData = await (await fetch(appConfig.api.config)).json();
    const globalConfig = {
        ...appConfig,
        ...configData
    };
    console.log({ type: "initial", globalConfig });
    return globalConfig;
};
console.log({ ENV, appConfig });

