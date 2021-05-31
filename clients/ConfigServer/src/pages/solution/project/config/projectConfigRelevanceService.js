import service from "@/utils/service";
import { request } from "umi";

const httpClient = service('project/config/relevance');


httpClient.queryPaging = async function (params) {
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

