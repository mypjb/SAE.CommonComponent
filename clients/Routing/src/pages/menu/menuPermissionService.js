import { request } from "umi";
import service from "@/utils/service";

const httpClient = service('menu/permission');
httpClient.delete = async function (data) {
    
    return request('/menu/permission', { method: "delete", data });
}
export default httpClient;

