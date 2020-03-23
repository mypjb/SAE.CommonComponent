import { defineConfig } from 'umi';

export default defineConfig({
  layout: {
    name:"SAE"
  },
  qiankun: {
    master: {
      // 注册子应用信息
      apps: [
        {
          name: 'config', // 唯一 id
          entry: '//localhost:8001', // html entry
          base: '/config', // app1 的路由前缀，通过这个前缀判断是否要启动该应用，通常跟子应用的 base 保持一致
        }
      ],
      jsSandbox: true, // 是否启用 js 沙箱，默认为 false
      prefetch: true, // 是否启用 prefetch 特性，默认为 true
    },
  }
});
