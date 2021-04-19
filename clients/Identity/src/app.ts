import { RequestConfig } from 'umi';

let masterProps = {};
export const qiankun = {
  // 应用加载之前
  async bootstrap(props) {
    masterProps = props;
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

const requestConfig = {
    credentials: "include",
    errorConfig: {
        adaptor: function (resData, context) {
            if (resData === context.res) {
                return {
                    ...resData,
                    success: true
                }
            }
            return {
                ...resData,
                success: false,
                errorMessage: resData.message || resData.title || resData.statusText,
            };
        }
    }
};

export async function getInitialState() {
    return {
        masterProps
      };
}


export const request: RequestConfig = requestConfig;

export const dva = {
    config: {
        onError(e) {
            e.preventDefault();
            console.error(e);
        },
    },
};

