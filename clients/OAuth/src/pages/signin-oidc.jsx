import { useLocation } from 'umi'
import oidc from 'oidc-client'
import { Fragment } from 'react';

export default ({ location }) => {
    const oidcConfig = {
        response_mode: "query"
    };

    const signinCallbackUrl = window.location.origin + window.location.pathname + (location.search || ('?' + location.hash.substr(1)));

    console.log(signinCallbackUrl);

    new oidc.UserManager(oidcConfig).signinRedirectCallback(signinCallbackUrl).then((user) => {
        window.localStorage.setItem("user",JSON.stringify(user));
        window.location.href="/";
    }).catch(e => {
        console.error(e);
    });

    return (<Fragment></Fragment>);
}