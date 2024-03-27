import service from "@/utils/service";
import { request } from "umi";

const httpClient = service('role/menu');

httpClient.tree = async function () {
    return request('/menu/tree', {
        method: "get"
    });
}
export default httpClient;
