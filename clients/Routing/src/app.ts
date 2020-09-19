import { RequestConfig } from 'umi';

export const request: RequestConfig = {
    prefix: "http://api.sae.com",
    credentials: "include",
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