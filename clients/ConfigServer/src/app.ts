import { RequestConfig } from 'umi';
let masterProps;
export const request: RequestConfig= {};
//     prefix: "http://api.sae.com",
//     credentials: "include",
//     requestInterceptors: [(url, options) => {
//         const userJson = localStorage.getItem("user");

//         const token = userJson ? JSON.parse(userJson).access_token : '';
//         console.info(token);
//         return {
//             url,
//             options: {
//                 ...options,
//                 headers: {
//                     Authorization: "Bearer " + token
//                 }
//             },
//         };
//     }],
//     errorConfig: {
//         adaptor: function (resData, context) {
//             if (resData === context.res) {
//                 return {
//                     ...resData,
//                     success: true
//                 }
//             }
//             return {
//                 ...resData,
//                 success: false,
//                 errorMessage: resData.message || resData.title || resData.statusText,
//             };
//         }
//     }
// };

export const qiankun = {
    // 应用加载之前
    async bootstrap(props) {
      masterProps = props;
      masterProps.initial(request);
 
      console.log({request});
      console.log('app1 bootstrap', props);
    },
    // 应用 render 之前触发
    async mount(props) {
      masterProps = props;
      
    },
    // 应用卸载之后触发
    async unmount(props) {
      console.log('app1 unmount', props);
    },
  };

export const dva = {
    config: {
        onError(e) {
            e.preventDefault();
            console.error(e);
        },
    },
};