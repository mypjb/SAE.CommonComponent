const ENV = process.env.UMI_ENV;

const callBackUrlKey = "sae_callback_url";

const userKey = "sae_user";

let configUrl = "http://api.sae.com/appdata/public?appid=ydEBPOQeVUOvpX4KeI9fIw&env=Production";

export const appConfig = {
    callBackUrlKey,
    callbackUrl: function () {
        const url = window.sessionStorage.getItem(callBackUrlKey);
        window.sessionStorage.removeItem(callBackUrlKey);
        return url || "/routing";
    }
};

if (ENV == "dev") {
    configUrl = "http://localhost:8080/appdata/public?appid=ydEBPOQeVUOvpX4KeI9fIw&env=Development";
}

export const userManager = {
    get: function () {
        const userJson = window.sessionStorage.getItem(userKey);
        try {
            if(userJson){
                return JSON.parse(userJson);
            }else{
             console.warn("尚未登陆！");   
            }
        } catch (e) {
            console.warn("用户信息解析无效！");
        }
        return null;
    },
    set: function (user) {
        if (user) {
            window.sessionStorage.setItem(userKey, JSON.stringify(user));
        } else {
            this.delete();
        }
    },
    delete: function () {
        window.sessionStorage.removeItem(userKey);
    }
}

export const load = async () => {
    const configData = await (await fetch(configUrl)).json();
    const globalConfig = {
        ...appConfig,
        ...configData,
        user:userManager.get(),
        userManager:userManager
    };

    return globalConfig;
};

