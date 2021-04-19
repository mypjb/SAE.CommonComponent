import { defineConfig } from 'umi';
import routes from './route';

export default defineConfig({
    layout: {},
    routes: routes,
    qiankun: {
        master: {
            jsSandbox: true, // 是否启用 js 沙箱，默认为 false
            prefetch: true, // 是否启用 prefetch 特性，默认为 true
        }
    }
});
