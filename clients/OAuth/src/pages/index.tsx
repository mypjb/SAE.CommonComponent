import React, { Fragment } from 'react';
import { UserManager } from 'oidc-client';
import { useModel } from 'umi';

export default (props) => {

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

  console.log(oidcConfig);

  const mgr = new UserManager(oidcConfig);

  mgr.createSigninRequest().then(request => {
    window.location.href = request.url;
  }).catch(val => {
    console.error(val);
  });

  return (<Fragment></Fragment>);
}
