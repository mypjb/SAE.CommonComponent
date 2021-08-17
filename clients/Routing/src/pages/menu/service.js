import { request } from "umi";
import service from "@/utils/service";

const httpClient = service('menu');

httpClient.tree = async function () {
    return request("/menu/tree");
};


export default httpClient;

