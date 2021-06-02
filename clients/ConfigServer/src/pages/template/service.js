import service from "@/utils/service";
import { request as httpClient } from "umi";

service.list = async function () {
  return request('/template/list')
}

export default service('template');

