import service from "@/utils/service";
import { request } from "umi";

const httpClient = service('project/config');

httpClient.delete = async function (data) {
    console.log(data);
    return request("/project/config", {
        method: "delete",
        data
    });
}

httpClient.queryReference = async function (params) {
    return request(`/project/config/paging`, {
        params
    });
}

httpClient.reference = async function (data) {
    return request(`/project/config`, {
        method:"post",
        data
    });
}

export default httpClient;

