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
    const { initialState } = useModel('@@initialState');
    const { setMasterState } = initialState;

    const mgr = new oidc.UserManager(oidcConfig);
    mgr.signinRedirectCallback(signinCallbackUrl).then((user) => {
        setMasterState({
            ...initialState,
            user
        });
        debugger;
        history.pushState(null,"","/config-server/template");
    }).catch(e => {
        console.error(e);
    });

    return (<Fragment>{initialState?.user}</Fragment>);
}