import service from "@/utils/service";
import { request } from "umi";

const httpClient = service('project/config/reference');

httpClient.reference = async function (data) {
  return request(`/project/config/reference`, {
        method:"post",
        data
    });
}

export default httpClient;

