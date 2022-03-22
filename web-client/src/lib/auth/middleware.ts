import staticAxios from 'axios'
import { apiAxios } from '@sentaku/lib'

import { ApplicationUser } from '../../models/user'

import type { GetServerSidePropsResult } from 'next/types'
import type { AuthConfig, AuthProps, Callback, GetServerSidePropsContextWithUser } from './types'

export const withAuth = <P>(callback: Callback<P>, config?: Partial<AuthConfig>) => {
  return async (context: GetServerSidePropsContextWithUser): Promise<GetServerSidePropsResult<AuthProps<P>>> => {
    const accessTokenName = process.env.COOKIE_ACCESS_TOKEN_NAME ?? ''

    const accessToken = context.req.cookies[accessTokenName]

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
            authError: {
              message: 'Forbidden',
              status: '403'
            }
          }
        }
      }
    }

    apiAxios.defaults.headers.common['Authorization'] = `Bearer ${accessToken}`

    try {
      if (context.req.user === undefined) {
        let {data: user} = await apiAxios.get<ApplicationUser>('/api/user')
        context.req = Object.defineProperty(context.req, 'user', {value: user})
      }

      const user = context.req.user as ApplicationUser

      if (config?.permissions && (user.permissions & config.permissions) !== config.permissions) {
        // TODO: add auth error
        return {
          redirect: {
            destination: '/',
            statusCode: 302
          }
        }
      }

      if (config?.roles && !config.roles.every(role => user.roles.some(userRole => userRole === role))) {
        // TODO: add auth error
        return {
          redirect: {
            destination: '/',
            statusCode: 302
          }
        }
      }

      const callbackResult = await callback(context)

      if ('props' in callbackResult) {
        if (callbackResult.props instanceof Promise) {
          return {
            props: callbackResult.props.then(result => ({
              user: context.req.user,
              ...result
            }))
          }
        }

        return {
          props: {
            user: context.req.user,
            ...callbackResult.props
          }
        }
      }

      return callbackResult

    } catch (err) {
      if (staticAxios.isAxiosError(err)) {
        return {
          props: {
            authError: {
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
