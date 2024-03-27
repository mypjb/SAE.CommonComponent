import { Select } from 'antd';

export default (props) => {
  const options = [
    { label: '<', value: '<' },
    { label: '>', value: '>' },
    { label: '==', value: '==' },
    { label: '<=', value: '<=' },
    { label: '>=', value: '>=' },
    { label: '!', value: '!' },
    { label: '!=', value: '!=' },
    { label: 'regex', value: 'regex' },
    { label: 'in', value: 'in' },
  ];
  return <Select options={options} {...props} />;
};
