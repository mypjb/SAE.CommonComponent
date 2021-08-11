import { defineConfig } from 'umi';

export default defineConfig({
    qiankun: { slave: {} },
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
            path: "/cluster/:id",
            component: "app"
        },
        {
            path: "/client/:id",
            component: "client"
        }
    ]
});
