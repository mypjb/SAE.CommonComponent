import service from "@/utils/service";
import { request as httpClient } from "umi";

const request = service('template');

request.list = async function () {
  return httpClient('/template/list')
}

export default request;

