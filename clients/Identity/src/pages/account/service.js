import { request } from "umi";

const httpClient = {};

httpClient.login = async function (data) {
    return request("/account/login", {
        method: "post",
        data
    });
};

export default httpClient;

