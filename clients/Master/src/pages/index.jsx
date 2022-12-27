import { MicroApp, useModel, useLocation } from 'umi';

export default () => {

  const { initialState } = useModel('@@initialState');
  const location = useLocation();
  debugger;
  const { apps } = initialState;
  console.log(apps);
  const app = apps.find(s => (s.path.toLowerCase() == location.pathname.toLowerCase() || location.pathname.toLowerCase().startsWith((s.path.toLowerCase() + '/'))));
  return (<MicroApp name='BasicData' basename="/basicdata" />);
  if (app) {
    return (<MicroApp name='BasicData' basename="/basicdata" />)
  } else {
    return (<div>not find app</div>)
  }
}
