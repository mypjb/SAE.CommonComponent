import request from "@/utils/request";

export async function queryPaging(params) {
    return request('/solution/paging',{
        params
    });
}