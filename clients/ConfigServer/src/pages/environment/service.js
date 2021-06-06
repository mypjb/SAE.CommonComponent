import service from "@/utils/service";
import { request as httpClient } from "umi";

const request = service('env');

request.list = async function () {
  return httpClient('/env/list')
}

export default request;

