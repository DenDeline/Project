import {
  GetServerSidePropsContext,
  GetServerSidePropsResult
} from 'next'

import { ApplicationUser } from '../models/user'
import { Permissions } from '@sentaku/constants'

import { apiAxios } from './defaults'
import staticAxios from 'axios'
export interface AuthConfig {
  withRedirect: boolean,
  roles: string[],
  permissions: Permissions
}

export interface AuthProps<P = {}> {
  data?: { user: ApplicationUser } & P
  error?: {
    status?: string,
    message: string
  }
}

export type GetServerSidePropsContextWithUser = GetServerSidePropsContext & { req: { user?: ApplicationUser } }
export type Callback<P> = (context: GetServerSidePropsContextWithUser) => Promise<GetServerSidePropsResult<P>>


export const withAuth = <P>(callback: Callback<P>, config?: Partial<AuthConfig>) => {
  return async (context: GetServerSidePropsContextWithUser): Promise<GetServerSidePropsResult<AuthProps<P>>> => {
    try {
      const accessToken = context.req.cookies.access_token

      if (!accessToken) {
        //TODO: Implement refresh token
        context.req.user = undefined
        if (config?.withRedirect) {
          return {
            redirect: {
              destination: '/sign-in',
              statusCode: 302
            }
          }
        } else {
          return {
            props: {
              error: {
                message: 'Forbidden',
                status: '403'
              }
            }
          }
        }
      }

      apiAxios.defaults.headers.common['Authorization'] = `Bearer ${accessToken}`

      if (context.req.user === undefined) {
        let {data: user} = await apiAxios.get<ApplicationUser>('/user')
        context.req = Object.defineProperty(context.req, 'user', {value: user})
      }

      const callbackResult = await callback(context)

      if ('props' in callbackResult) {
        return {
          props: {
            data: {
              user: context.req.user as ApplicationUser,
              ...callbackResult.props
            }
          }
        }
      }

      return callbackResult

    } catch (err) {
      if (staticAxios.isAxiosError(err)) {
        return {
          props: {
            error: {
              status: err.code,
              message: err.message
            }
          }
        }
      }

      throw err
    }
  }
}
