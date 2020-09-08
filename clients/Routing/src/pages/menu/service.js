import { request } from "umi";
import service from "@/utils/service";

const httpClient = service('menu');

httpClient.list = async function () {
    return request("/menu/all");
};

httpClient.remove = async function (data) {
    return request("/menu",{
        method:"delete",
        data
    });
};

export default httpClient;

