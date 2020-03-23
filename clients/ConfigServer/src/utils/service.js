import { request } from "umi";

export default function (action) {
    const service = {};

    service.query = async function (id) {
        return request(`/${action}/${id}`);
    }

    service.queryPaging = async function (params) {
        return request(`/${action}/paging`, {
            params
        });
    }

    service.add = async function (data) {
        return request(`/${action}`, {
            method: "post",
            data
        });
    }

    service.edit = async function (data) {
        return request(`/${action}`, {
            method: "put",
            data
        });
    }

    service.remove = async function (id) {
        return request(`/${action}/${id}`, { method: "delete" });
    }

    return service;
}