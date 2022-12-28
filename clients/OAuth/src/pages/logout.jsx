import React, { Fragment } from 'react';
import { UserManager } from 'oidc-client-ts';
import { useModel } from 'umi';

export default () => {

    const { siteConfig } = useModel('@@initialState').initialState?.masterProps.masterState;
    const { authority, appId, redirectUris, postLogoutRedirectUris, scope } = siteConfig.oauth;

    const oidcConfig = {
        authority: authority,
        client_id: appId,
        redirect_uri: Array.isArray(redirectUris) ? redirectUris[0] : redirectUris,
        response_type: "id_token token",
        scope: "openid profile " + scope,
        post_logout_redirect_uri: Array.isArray(postLogoutRedirectUris) ? postLogoutRedirectUris[0] : postLogoutRedirectUris
    };

    const mgr = new UserManager(oidcConfig);
    mgr.signoutRedirect().then(request => {
        window.location.href = request.url;
    }).catch(val => {
        console.error(val);
        Modal.error({
            title: '认证端点异常',
            content: '无法连接到认证端点，请联系管理员排查此问题!',
        })
    });
    return (<Fragment></Fragment>);
}
