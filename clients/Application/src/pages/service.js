import { request } from "umi";
import service from "@/utils/service";

const httpClient = service('app');

httpClient.refreshSecret = async function (data) {
    return request('/app/refresh/' + data, {
        method: "put"
    });
}

export default httpClient;

