import { RequestConfig } from 'umi';

export const qiankun = function () {
    return {
        // 注册子应用信息
        apps: [
            // {
            //   name: 'config', // 唯一 id
            //   entry: '//dev.sae.com:8001', // html entry
            // },
            {
                name: 'account', // 唯一 id
                entry: '//dev.sae.com:8002', // html entry
            },
            {
                name: 'oauth', // 唯一 id
                entry: '//dev.sae.com:8003', // html entry
            }
        ],
        lifeCycles: {
            afterMount: props => {
                console.log(props);
            },
        }
    }
}
const menuData = [
    {
        path: '/',
        name: 'welcome',
        icon: 'icon-smile',
        children: [
            {
                path: '/welcome',
                name: 'one',
                icon: 'icon-menu',
                children: [
                    {
                        path: '/welcome/welcome',
                        name: 'two',
                        exact: true,
                        icon: 'icon-user'
                    },
                ],
            },
        ],
    },
    {
        path: '/user',
        name: '用户',
        icon: 'icon-user'
    },
];

export const layout = {
    name: "SAE",
    menuDataRender: () => {
        debugger;
        return menuData;
    }
};

// export const dva = {
//   config: {
//     onError(e) {
//       e.preventDefault();
//       console.error(e.message);
//     },
//   }
// };
