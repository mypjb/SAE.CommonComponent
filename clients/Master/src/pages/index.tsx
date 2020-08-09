import React from 'react'
import { MicroApp } from 'umi'

const findApp = (pathname, routes) => {
  if (pathname && routes) {
    for (let i = 0; i < routes.length; i++) {
      const route = routes[i];
      if (pathname.toLowerCase().startsWith(route.path)) {
        return route;
      } else {
        if (route.routes) {
          return findApp(pathname, route.routes);
        }
      }
    }
  }
  return null;
}

const routeList = [{
  path: '/identity',
  microApp: 'identity'
},
{
  path: '/oauth',
  microApp: 'oauth'
}];

export default ({ location }) => {
  const app = findApp(location.pathname, routeList);
  const element = (app) ? (<MicroApp name={app.microApp} ></MicroApp>) : (<div>404</div>);
  return (
    <div>
      {element}
    </div>
  );
}
