import { MicroApp } from 'umi';
import apps from '../../config/app';

export default ({ match }) => {

  const { path } = match;

  const app = apps.find(s => (s.path == path || path.startsWith((s.path + '/'))));

  console.log({ path, app });

  if (app) {
    return (<MicroApp name={app.name} base={app.path} />)
  } else {
    return (<div>not find app</div>)
  }
}
