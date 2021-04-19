import { useLocation, useModel  } from 'umi'
import oidc from 'oidc-client'
import { Fragment } from 'react';

export default ({ location }) => {
    const oidcConfig = {
        response_mode: "query",
        loadUserInfo: false
    };

    const signinCallbackUrl = window.location.origin + window.location.pathname + (location.search || ('?' + location.hash.substr(1)));

    console.log(signinCallbackUrl);
    const { masterState,setMasterState,masterPush } = useModel('@@initialState').initialState?.masterProps;

    const mgr = new oidc.UserManager(oidcConfig);
    mgr.signinRedirectCallback(signinCallbackUrl).then((user) => {
        setMasterState({
            ...masterState,
            user
        });
        masterPush("/config/template");
    }).catch(e => {
        console.error(e);
    });

    return (<Fragment>{masterState?.user}</Fragment>);
}