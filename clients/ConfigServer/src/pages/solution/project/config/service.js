import service from "@/utils/service";
import { request } from "umi";

const httpClient = service('project/config');

httpClient.remove = async function (data) {
    console.log(data);
    return request("/project/config", {
        method: "delete",
        data
    });
}

httpClient.queryRelevance = async function (params) {
    return request(`/project/config/relevance`, {
        params
    });
}

httpClient.relevance = async function (data) {
    return request(`/project/config/relevance`, {
        method:"post",
        data
    });
}

export default httpClient;

