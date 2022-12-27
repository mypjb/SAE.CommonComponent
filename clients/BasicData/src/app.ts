import { RequestConfig, setCreateHistoryOptions } from 'umi';
let masterProps;
export const request: RequestConfig = {};
export const qiankun = {
    // 应用加载之前
    async bootstrap(props) {

        masterProps = props;

        if (masterProps && masterProps?.initial) {
            masterProps.initial(request);
        }

        const basename = props?.basename;
        if (basename) setCreateHistoryOptions({ basename });
    },
    // 应用 render 之前触发
    async mount(props) {
        masterProps = props;
    },
    // 应用卸载之后触发
    async unmount(props) {
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