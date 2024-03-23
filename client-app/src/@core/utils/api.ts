import axios from 'axios'

const fetchClient = () => {
  const baseUrl = process.env.NEXT_PUBLIC_BLOG_APP_BASE_URL
  const defaultOptions = {
    baseURL: baseUrl,
    headers: {
      'Content-Type': 'application/json',
      'Access-Control-Allow-Origin': '*'
    }
  }

  // Create instance
  let instance = axios.create(defaultOptions)

  // Set the AUTH token for any request
  instance.interceptors.request.use(function (config) {
    const token = localStorage.getItem('jwt')
    config.headers.Authorization = token ? `Bearer ${token}` : ''
    return config
  })

  return instance
}

export default fetchClient()
