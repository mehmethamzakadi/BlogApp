import axios from 'axios'
import { useSnackbar } from 'notistack'

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

  instance.interceptors.response.use(
    function (response) {
      console.log(`Response: ${response.status} ${response.config.url}`)
      return response
    },
    function (error) {
      console.log(`Error: ${error.response.status} ${error.config.url}`)
      return Promise.reject(error)
    }
  )

  return instance
}

export default fetchClient()
