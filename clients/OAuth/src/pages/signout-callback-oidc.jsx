import { useModel } from 'umi'
export default () => {
    const { masterState, setMasterState, masterPush } = useModel('@@initialState').initialState?.masterProps;
    masterPush.push("/");
    return (<></>);
}