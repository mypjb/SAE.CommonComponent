import React, { useState, Fragment } from 'react'
import { MicroApp } from 'umi'
import { format } from 'prettier';

const findApp = (pathname, routes) => {
  if (pathname && routes) {
    for (let i = 0; i < routes.length; i++) {
      const route = routes[i];
      if (pathname.toLowerCase().startsWith(route.path)) {
        return <MicroApp key={route.microApp} name={route.microApp}></MicroApp>;
      } else {
        if (route.routes) {
          return findApp(pathname, route.routes);
        }
      }
    }
  }
  return <Fragment key={401}></Fragment>;
}

const routeList = [{
  path: '/identity',
  microApp: 'identity'
},
{
  path: '/oauth',
  microApp: 'oauth'
},
{
  path: '/config-server',
  microApp: 'config-server'
},
{
  path: '/routing',
  microApp: 'routing'
}];

export default ({ location }) => {

  const [element, setElement] = useState(0);

  const app = findApp(location.pathname, routeList);

  if (element === 0 || element.key !== app.key) {
    setElement(app);
  }
  
  return element;
}
