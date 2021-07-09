import { request } from "umi";
import service from "@/utils/service";

const httpClient = service('plugin');

httpClient.edit = async function (data) {
    return request('/plugin/entry', {
        method: "put",
        data
    });
}

export default httpClient;

