import service from "@/utils/service";
import { request } from "umi";

const httpClient = service('app/config/reference');

httpClient.reference = async function (data) {
  return request(`/app/config`, {
        method:"post",
        data
    });
}

export default httpClient;

