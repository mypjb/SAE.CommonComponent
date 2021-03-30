export default [
    {
      path: '/',
      component:"index"
    },
    {
      path: '/identity',
      name: 'identity',
      microApp: 'identity'
    },
    {
      path: '/oauth',
      name: 'Login',
      microApp: 'oauth'
    },
    {
      path: '/config-server',
      name: 'config',
      microApp: 'config-server'
    },
    {
      path: '/routing',
      name: 'routing',
      microApp: 'routing'
    }
];