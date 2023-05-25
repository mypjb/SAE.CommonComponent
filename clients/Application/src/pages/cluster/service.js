import service from "@/utils/service";
import { request } from "umi";

const httpClient = service('cluster');

httpClient.find = async function (id) {
    return request(`/cluster?id=${id}`);
}

export default httpClient;

