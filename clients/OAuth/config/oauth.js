import { useModel } from 'umi';
export default () => {
    const initialState = useModel('@@initialState').initialState;

    const { siteConfig } = initialState.masterProps.masterState;

    const { authority, appId, redirectUris, postLogoutRedirectUris, scope } = siteConfig.oauth;

    const oidcConfig = {
        authority: authority,
        client_id: appId,
        redirect_uri: Array.isArray(redirectUris) ? redirectUris[0] : redirectUris,
        response_type: "code",
        scope: "openid profile " + scope,
        post_logout_redirect_uri: Array.isArray(postLogoutRedirectUris) ? postLogoutRedirectUris[0] : postLogoutRedirectUris
    };

    console.log(oidcConfig);

    return oidcConfig;
}