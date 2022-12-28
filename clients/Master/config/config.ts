import { defineConfig } from 'umi';
export default defineConfig({
    layout: {},
    dva: {},
    initialState: {},
    model: {},
    request: {},
    antd: {},
    qiankun: {
        master: {
            jsSandbox: true, // 是否启用 js 沙箱，默认为 false
            prefetch: false, // 是否启用 prefetch 特性，默认为 true
        }
    }
});
