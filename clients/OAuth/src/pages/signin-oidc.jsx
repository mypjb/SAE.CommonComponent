import { useLocation, useModel } from 'umi';
import { UserManager } from 'oidc-client-ts';
import oidcConfig from '@/../config/oauth';
import { Fragment } from 'react';

export default () => {

    const config = oidcConfig();

    const location = useLocation();

    const signinCallbackUrl = window.location.origin + window.location.pathname + (location.search || ('?' + location.hash.substr(1)));

    const { masterState, setMasterState, masterPush } = useModel('@@initialState').initialState?.masterProps;

    const { callbackUrl,userManager } = masterState;

    const mgr = new UserManager(config);
    mgr.signinRedirectCallback(signinCallbackUrl).then((user) => {
        
        userManager.set(user);
        setMasterState({
            ...masterState,
            user
        });
        masterPush(callbackUrl());
    }).catch(e => {
        console.error(e);
    });

    return (<Fragment>{masterState?.user}</Fragment>);
}