import { RequestConfig } from 'umi';
let masterProps = {};
export const request: RequestConfig = {};
export const qiankun = {
    // 应用加载之前
    async bootstrap(props) {
        masterProps = props;
        masterProps?.initial(request);
        if (request.middlewares) {
            delete request.middlewares;
        }
        console.log(request);
    },
    // 应用 render 之前触发
    async mount(props) {
        masterProps = props;
    },
    // 应用卸载之后触发
    async unmount(props) {
    },
};

export async function getInitialState() {
    return {
        masterProps
    };
}

export const dva = {
    config: {
        onError(e) {
            e.preventDefault();
            console.error(e);
        },
    },
};