import service from "@/utils/service";
import { request } from "umi";


const httpClient = service('project');

httpClient.publish = async function (data) {
    return request(`/project/publish`, {
        method: "post",
        data
    });
}

httpClient.preview = async function (data) {
    return request(`/project/preview`, {
        method: "get",
        data
    });
}


export default httpClient;

