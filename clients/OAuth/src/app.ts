import { useModel } from "umi";
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

export async function getInitialState() {
  return {
    masterProps
  };
}