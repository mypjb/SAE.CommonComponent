import { RequestConfig } from 'umi';

export const request: RequestConfig = {
    prefix: "http://localhost:5000",
    errorConfig: {
        adaptor: (resData, context) => {
            if (resData.statusCode === 0) {
                context.res = resData.body;
            }
            return {
                ...resData,
                success: resData.statusCode === 0,
                errorMessage: resData.message,
            };
        },
    }
};

export const dva = {
    config: {
        onError(e) {
            e.preventDefault();
            console.error(e.message);
        },
    }
};