import request from "@/utils/request";

export default function (action) {
    const service = {};

    service.query = async function (id) {
        return request.get(`/${action}/${id}`);
    }

    service.queryPaging = async function (params) {
        return request.get(`/${action}/paging`, {
            params
        });
    }

    service.add = async function (data) {
        return request.post(`/${action}`, {
            data
        });
    }

    service.edit = async function (data) {
        return request.put(`/${action}`, {
            data
        });
    }

    service.remove = async function (id) {
        return request.delete(`/${action}/${id}`);
    }

    return service;
}


