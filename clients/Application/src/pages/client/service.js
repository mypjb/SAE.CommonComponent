import { request } from "umi";
import service from "@/utils/service";

const httpClient = service('client');

httpClient.refreshSecret = async function (data) {
    return request('/client/refresh/' + data, {
        method: "put"
    });
}

export default httpClient;

