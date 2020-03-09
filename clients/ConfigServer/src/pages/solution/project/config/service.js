import service from "@/utils/service";
import request from "@/utils/request";

const httpClient = service('project/config');

httpClient.remove = async function (data) {
    console.log(data);
    return request.delete("/project/config", { data });
}

httpClient.queryRelevance = async function (params) {
    return request.get(`/project/config/relevance`, {
        params
    });
}

httpClient.relevance = async function (data) {
    return request.post(`/project/config/relevance`, {
        data
    });
}

export default httpClient;

