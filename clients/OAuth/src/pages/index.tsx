import React, { Fragment } from 'react';
import { UserManager } from 'oidc-client';
import { useRequest } from 'umi';
const oidcConfig = {
  authority: "http://identity.sae.com:8080",
  client_id: "localhost.test",
  redirect_uri: "http://dev.sae.com:8000/oauth/signin-oidc",
  response_type: "id_token token",
  scope: "openid profile api",
  post_logout_redirect_uri: "http://dev.sae.com:8000",
};
const mgr = new UserManager(oidcConfig);

export default () => {
  mgr.createSigninRequest().then(request => {
    window.location.href = request.url;
  }).catch(val => {
    console.error(val);
  });
  return (<Fragment>11111111111</Fragment>);
}
