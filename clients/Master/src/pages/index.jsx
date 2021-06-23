import { MicroApp } from 'umi';
import apps from '../../config/app';

export default ({ match }) => {

  const { path } = match;

  const app = apps.find(s => (s.path == path || (s.path + '/').startsWith(path)));

  console.log({ path, app });

  if (app) {
    return (<MicroApp name={app.name} />)
  } else {
    return (<div>not find app</div>)
  }

}
