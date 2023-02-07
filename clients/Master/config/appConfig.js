const ENV = process.env.UMI_ENV;

const callBackUrlKey = "saeCallbackUrl";

let configUrl = "http://api.sae.com/appdata/public?appid=vCSuqdHsWEyGZzEW1NKaFg&env=Production";

export const appConfig = {
    callBackUrlKey,
    callbackUrl: function () {
        const url = window.sessionStorage.getItem(callBackUrlKey);
        window.sessionStorage.removeItem(callBackUrlKey);
        return url || "/routing";
    }
};

if (ENV == "dev") {
    configUrl = "http://localhost:8080/appdata/public?appid=vCSuqdHsWEyGZzEW1NKaFg&env=Development";
}

export const load = async () => {
    const configData = await (await fetch(configUrl)).json();
    const globalConfig = {
        ...appConfig,
        ...configData
    };

    return globalConfig;
};

