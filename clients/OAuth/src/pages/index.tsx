import oidcConfig from '@/../config/oauth';
import { UserManager } from 'oidc-client-ts';
import { Fragment } from 'react';

export default () => {
  
  const config = oidcConfig();
  
  const mgr = new UserManager(config);

  mgr.signinRedirect()
     .then((request) => {
       window.location.href = request.url;
     })
     .catch((val) => {
       console.error(val);
     });

  return <Fragment></Fragment>;
};
