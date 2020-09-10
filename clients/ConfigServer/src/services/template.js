import { request as httpClient } from "umi";

const request = {};

request.list = async function () {
  return httpClient('/template/list')
}

export default request;
