import service from "@/utils/service";
import { request } from "umi";

const httpClient = service('app/config');

httpClient.delete = async function (data) {
    console.log(data);
    return request("/app/config", {
        method: "delete",
        data
    });
}

httpClient.queryReference = async function (params) {
    return request(`/app/config/paging`, {
        params
    });
}

httpClient.reference = async function (data) {
    return request(`/app/config`, {
        method:"post",
        data
    });
}

export default httpClient;

