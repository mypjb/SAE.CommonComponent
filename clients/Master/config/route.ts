export default [
  {
    path: '/',
    component: "index",
    name: "home",
    hideInMenu: true,
  },
  {
    path: '/identity',
    name: 'identity',
    microApp: 'identity',
    hideInMenu: true,
    headerRender: false,//dependencie @ant-design/pro-layout package
    menuRender: false,
    menuHeaderRender: false
  },
  {
    path: '/oauth',
    name: 'Login',
    microApp: 'oauth',
    hideInMenu: true,
    headerRender: false,
    menuRender: false,
    menuHeaderRender: false
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