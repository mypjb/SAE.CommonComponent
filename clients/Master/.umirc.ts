import { defineConfig } from 'umi';

export default defineConfig({
  layout: {
    name: "SAE"
  },
  routes: [
    {
      path: '/',
      routes: [
        {
          path: '/identity',
          microApp: 'identity'
        },
        {
          path: '/oauth',
          microApp: 'oauth'
        }
      ]
    },
  ],
  qiankun: {
    master: {
      // 注册子应用信息
      apps: [
        // {
        //   name: 'config', // 唯一 id
        //   entry: '//dev.sae.com:8001', // html entry
        // },
        {
          name: 'identity', // 唯一 id
          entry: '//dev.sae.com:8002', // html entry
        },
        {
          name: 'oauth', // 唯一 id
          entry: '//dev.sae.com:8003', // html entry
        }
      ],
      jsSandbox: true, // 是否启用 js 沙箱，默认为 false
      prefetch: true, // 是否启用 prefetch 特性，默认为 true
    }
  }
});
