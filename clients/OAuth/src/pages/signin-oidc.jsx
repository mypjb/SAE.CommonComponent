import { useLocation } from 'umi'
import oidc from 'oidc-client'

export default () => {
    new oidc.UserManager({ response_mode: "query" }).signinRedirectCallback().then(user => {
        console.log(user);
    }).catch(e => {
        console.error(e);
    });

    return (<Fragment></Fragment>);
}