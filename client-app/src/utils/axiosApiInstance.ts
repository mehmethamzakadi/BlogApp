import axios from "axios";

const fetchClient = () => {
  const defaultOptions = {
    baseURL: "https://localhost:7285/",
    // method: "get",
    headers: {
      "Content-Type": "application/json",
    },
  };

  // Create instance
  let instance = axios.create(defaultOptions);

  // Set the AUTH token for any request
  instance.interceptors.request.use(function (config) {
    debugger;
    const token = localStorage.getItem("jwt");
    config.headers.Authorization = token ? `Bearer ${token}` : "";
    return config;
  });

  return instance;
};

export default fetchClient();