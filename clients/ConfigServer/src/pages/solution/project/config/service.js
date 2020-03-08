import service from "@/utils/service";
import request from "@/utils/request";

const httpClient = service('project/config');

httpClient.queryRelevance = async function (params) {
    return request.get(`project/config/relevance`, {
        params
    });
}

export default httpClient;

