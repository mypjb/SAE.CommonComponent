import { RequestConfig } from 'umi';

export const request: RequestConfig = {
    prefix: "http://api.sae.com",
    credentials: "include",
    requestInterceptors: [(url, options) => {
        const userJson = localStorage.getItem("user");

        const token = userJson ? JSON.parse(userJson).access_token : '';
        console.info(token);
        return {
            url,
            options: {
                ...options,
                headers: {
                    Authorization: "Bearer " + token
                }
            },
        };
    }],
    errorConfig: {
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
    }
};

export const dva = {
    config: {
        onError(e) {
            e.preventDefault();
            console.error(e);
        },
    },
};