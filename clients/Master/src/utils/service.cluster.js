import service from "./service";
import { request } from "umi";

const api = "cluster";

const httpClient = service(api);

httpClient.list = async () => {
    return request(`/${api}/list`);
}

httpClient.app = {
    list: async (clusterId) => {
        return request(`/${api}/app/list`, {
            params: clusterId
        });
    }
}

export default httpClient;
