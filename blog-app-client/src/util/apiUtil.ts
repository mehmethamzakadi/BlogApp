import axios from 'axios';
import { getToken } from './tokenUtil';

const fetchClient = () => {
  const baseUrl = process.env.REACT_APP_BLOG_APP_BASE_URL;
  const defaultOptions = {
    baseURL: baseUrl,
    headers: {
      'Content-Type': 'application/json',
      'Access-Control-Allow-Origin': '*'
    }
  };

  // Create instance
  let instance = axios.create(defaultOptions);

  // Set the AUTH token for any request
  instance.interceptors.request.use(function (config) {
    const token = getToken();
    config.headers.Authorization = token ? `Bearer ${token}` : '';
    return config;
  });

  return instance;
};

export default fetchClient();
