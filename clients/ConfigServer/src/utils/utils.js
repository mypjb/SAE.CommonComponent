import { parse } from 'querystring';
import pathRegexp from 'path-to-regexp';

export const validatorJson = (rule, value) => {
  if (value) {
    try {
      eval(`(${value})`);
    } catch (e) {
      console.error(e);
      return Promise.reject('this is json invalid');
    }
  }
  return Promise.resolve();
}


export const handleFormat = function ({ form, fieldName }, e) {
  const value = e.target.value;
  if (value) {
    try {
      const json = eval('(' + value + ')');
      const data={};
      data[fieldName]=JSON.stringify(json, null, 4);
      form.setFieldsValue(data);
    } catch{ }
  }
};