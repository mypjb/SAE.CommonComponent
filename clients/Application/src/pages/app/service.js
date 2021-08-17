import service from "@/utils/service";
import { request } from "umi";


const httpClient = service('cluster/app');

httpClient.publish = async function (data) {
    return request(`/app/config/publish`, {
        method: "post",
        data
    });
}

httpClient.preview = async function (params) {
    return request(`/app/config/preview`, {
        method: "get",
        params
    });
}

httpClient.appConfig = async function (params) {
    return request(`/cluster/app/config`, {
        method: "get",
        params
    });
}


export default httpClient;

