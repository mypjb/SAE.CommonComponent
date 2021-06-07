import service from "@/utils/service";
import { request } from "umi";

const httpClient = service('project/config/relevance');

httpClient.relevance = async function (data) {
    return request(`/project/config/relevance`, {
        method:"post",
        data
    });
}

export default httpClient;

