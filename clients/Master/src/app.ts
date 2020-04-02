
const handleRoute = function (rootRoute, routes) {
  
  if (routes === null || !routes.length) {
    return;
  }

  routes.forEach(element => {
    
    if(element.routes){
      handleRoute(rootRoute, element.routes);
    }
    
    if (element.path === rootRoute.path) {
      return;
    }
    element.component=rootRoute.component;
  });
}

export function patchRoutes({ routes }) {
  
  const routeList = routes[0].routes;
  const rootRoute = routeList.find(element => (element.path === "/"));
  console.log(routes,rootRoute);
  handleRoute(rootRoute,routes);
  
}

export const dva = {
  config: {
      onError(e) {
          e.preventDefault();
          console.error(e.message);
      },
  }
};
