const ENV = process.env.UMI_ENV;

const callBackUrlKey = "saeCallbackUrl";

let configUrl = "http://api.sae.com/appdata/public?env=Production&appid=localhost.test.app";

export const appConfig = {
    callBackUrlKey,
    callbackUrl: function () {
        const url = window.sessionStorage.getItem(callBackUrlKey);
        window.sessionStorage.removeItem(callBackUrlKey);
        return url || "/routing";
    }
};

if (ENV == "prod") {
    configUrl += "&env=Production";
} else {
    configUrl += "&env=Development";
}

export const load = async () => {
    const configData = await (await fetch(configUrl)).json();
    const globalConfig = {
        ...appConfig,
        ...configData
    };
    globalConfig
    console.log({ type: "initial", globalConfig });
    return globalConfig;
};
console.log({ ENV, appConfig });

