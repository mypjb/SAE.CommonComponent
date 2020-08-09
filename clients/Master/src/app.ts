import { RequestConfig } from 'umi';


const menuData = [
    {
        path: '/oauth',
        name: 'login',
        icon: 'icon-smile',
        // children: [
        //     {
        //         path: '/welcome',
        //         name: 'one',
        //         icon: 'icon-menu',
        //         children: [
        //             {
        //                 path: '/welcome/welcome',
        //                 name: 'two',
        //                 exact: true,
        //                 icon: 'icon-user'
        //             },
        //         ],
        //     },
        // ],
    }
];

export const qiankun = function () {
    return {
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
        lifeCycles: {
            afterMount: props => {
                console.log(props);
            },
        }
    }
};

export const layout = {
    name: "SAE",
    menuDataRender: () => {
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
