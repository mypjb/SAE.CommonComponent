const ENV = process.env.UMI_ENV;

const callBackUrlKey = "sae_callback_url";

const userKey = "sae_user";

const clusterKey = "_cluster_selected_key_";

let configUrl = "http://api.sae.com/appdata/public?appid=e1Bd2ZRzhkCFMsJGfXwgCw&env=Production";


const storage = window.localStorage;

export const appConfig = {
    callBackUrlKey,
    callbackUrl: function () {
        const url = window.sessionStorage.getItem(callBackUrlKey);
        window.sessionStorage.removeItem(callBackUrlKey);
        return url || "/routing";
    }
};

if (ENV == "dev") {
    configUrl = "http://localhost:8080/appdata/public?appid=e1Bd2ZRzhkCFMsJGfXwgCw&env=Development";
}

export const userManager = {
    get: function () {
        const userJson = storage.getItem(userKey);
        try {
            if (userJson) {
                return JSON.parse(userJson);
            } else {
                console.warn("尚未登陆！");
            }
        } catch (e) {
            console.warn("用户信息解析无效！");
        }
        return null;
    },
    set: function (user) {
        if (user) {
            storage.setItem(userKey, JSON.stringify(user));
        } else {
            this.delete();
        }
    },
    delete: function () {
        storage.removeItem(userKey);
    }
}

export const clusterManager = {
    get: function (userInfo) {
        const json = storage.getItem(clusterKey);
        if (json) {
            try {
                const data = JSON.parse(json);
                if (data.userId == userInfo.sub) {
                    return data.values;
                }
            } catch (e) {
                console.error("selected cluster parse fail");
            }
        }
        //保存的选项不是上一个账号所流。
        storage.removeItem(clusterKey);

        return [];
    },
    set: function (values, userInfo) {
        if (values && values.length) {
            storage.setItem(clusterKey, JSON.stringify({
                userId: userInfo.sub,
                values: values
            }));
        }
    }
}

export const load = async () => {
    const configData = await (await fetch(configUrl)).json();
    const user = userManager.get();

    let globalData = {};
    console.log(user);
    if (user) {
        const clusterValues = clusterManager.get(user.profile);

        globalData = {
            clusterId: clusterValues.length > 0 ? clusterValues[0] : "",
            appId: clusterValues.length > 1 ? clusterValues[1] : ""
        };
    }


    const globalConfig = {
        ...appConfig,
        ...configData,
        user: user,
        userManager: userManager,
        globalData
    };

    return globalConfig;
};
