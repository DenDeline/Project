import axios from 'axios'

export const apiAxios = axios.create({
  baseURL: process.env.BACKEND_API_URL
})

export const authAxios = axios.create({
  baseURL: process.env.AUTH_SERVER_URL
})
