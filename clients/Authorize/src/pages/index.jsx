import { history } from 'umi';

export default () => {
    fetch("http://dev.sae.com:8005")
        .then((rep) => {
            console.log(rep);
        });
    return (<div>Authorize</div>)
}