import { defineConfig } from 'umi';

export default defineConfig({
    qiankun: { slave: {} },
    dva:{},
    initialState:{},
    model:{},
    request:{},
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
            path: "/cluster/app/:id",
            component: "app"
        },
        {
            path: "/cluster/app/client/:id",
            component: "client"
        },
        {
            path: "/cluster/config/:id",
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
