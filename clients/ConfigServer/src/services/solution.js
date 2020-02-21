import request from "@/utils/request";

export async function query(params) {
    return request.get('/solution/' + params);
}

export async function queryPaging(params) {
    return request.get('/solution/paging', {
        params
    });
}

export async function add(params) {
    return request.post('/solution/add', {
        params
    });
}

export async function edit(params) {
    return request.put('/solution/edit', {
        params
    });
}

export async function remove(params) {
    return request.delete('/solution/' + params);
}

