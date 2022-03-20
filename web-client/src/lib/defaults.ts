import axios from 'axios'

export const apiAxios = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL
})

export const authAxios = axios.create({
  baseURL: process.env.NEXT_PUBLIC_AUTH_SERVER_URL
})
