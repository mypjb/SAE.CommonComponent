export default [
  {
    path: '/',
    component: "index",
    name: "home",
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
    path: '/config',
    name: 'config',
    microApp: 'config-server'
  },
  {
    path: '/routing',
    name: 'routing',
    microApp: 'routing'
  }
];