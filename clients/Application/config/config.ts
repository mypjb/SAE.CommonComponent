import { defineConfig } from 'umi';

export default defineConfig({
    qiankun: { slave: {} },
    dva: {},
    initialState: {},
    model: {},
    request: {},
    antd: {},
    layout: false,
    routes: [
        {
            path: '/',
            component: "index"
        },
        {
            path: '/cluster',
            component: 'cluster'
        },
        {
            path: "/cluster/app/:clusterId",
            component: "app"
        },
        {
            path: "/cluster/app/client/:appId",
            component: "client"
        },
        {
            path: "/cluster/config/:clusterId",
            component: "config"
        },
        {
            path:"/cluster/app/config/:id",
            component:"appConfig"
        },
        {
            path: "/template",
            component: "template"
        }
    ]
});
