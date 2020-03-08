import httpClient from '@/utils/request';

const request = {};

request.list = async function () {
  return httpClient('/template/list')
}

export default request;
