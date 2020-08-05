import React, { Fragment } from 'react';
import styles from './index.less';
import { UserManager } from 'oidc-client'

export default () => {
  const oidcConfig = {
    authority: "http://sae.com:8002",
    client_id: "localhost.test",
    redirect_uri: "http://dev.sae.com:8000/signin-oidc",
    response_type: "id_token token",
    scope: "openid profile api",
    post_logout_redirect_uri: "http://dev.sae.com:8000",
  };
  const mgr = new UserManager(oidcConfig);
  mgr.signoutRedirect();
  
  return (<Fragment></Fragment>);
}
