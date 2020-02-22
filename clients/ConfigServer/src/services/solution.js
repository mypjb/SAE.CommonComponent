import request from "@/utils/request";

export async function query(params) {
    return request.get('/solution/' + params);
}

export async function queryPaging(params) {
    return request.get('/solution/paging', {
        params
    });
}

export async function add(data) {
    return request.post('/solution', {
        data
    });
}

export async function edit(data) {
    return request.put('/solution', {
        data
    });
}

export async function remove(params) {
    return request.delete('/solution/' + params);
}

