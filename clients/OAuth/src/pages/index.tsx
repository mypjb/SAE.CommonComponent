import React, { Fragment } from 'react';
import { UserManager } from 'oidc-client';
import { useModel } from 'umi';

export default (props) => {

  const { siteConfig } = useModel('@@initialState').initialState?.masterProps.masterState;
  const { authority, appId, redirectUris, postLogoutRedirectUris } = siteConfig;

  const oidcConfig = {
    authority: authority,
    client_id: appId,
    redirect_uri: redirectUris,
    response_type: "id_token token",
    scope: "openid profile " + siteConfig.scope,
    post_logout_redirect_uri: postLogoutRedirectUris,
  };

  const mgr = new UserManager(oidcConfig);

  mgr.createSigninRequest().then(request => {
    window.location.href = request.url;
  }).catch(val => {
    console.error(val);
  });

  return (<Fragment></Fragment>);
}
