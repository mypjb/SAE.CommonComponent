import { request } from "umi";

export default function (action) {
    const service = {};

    service.find = async function (id) {
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

    service.status = async function (data) {
        return request(`/${action}/status`, {
            method: "put",
            data
        });
    }

    service.delete = async function (data) {
        if (data.id) {
            return request(`/${action}/${data.id}`, { method: "delete" });
        } else {
            return request(`/${action}`, { method: "delete", data });
        }

    }



    return service;
}