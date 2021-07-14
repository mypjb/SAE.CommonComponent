import { MicroApp, useModel } from 'umi';

export default ({ match }) => {

  const { initialState } = useModel('@@initialState');
  const { path } = match;

  const { apps } = initialState;
  const app = apps.find(s => (s.path == path || path.startsWith((s.path + '/'))));

  console.log({ path, app, initialState });

  if (app) {
    return (<MicroApp name={app.name} base={app.path} autoSetLoading />)
  } else {
    return (<div>not find app</div>)
  }
}
