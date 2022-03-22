import { ApplicationUser } from 'models/user'
import type { GetServerSidePropsContext, GetServerSidePropsResult } from 'next/types'

import type { Permissions } from '@sentaku/constants'

export interface AuthConfig {
  withRedirect: boolean,
  roles: string[],
  permissions: Permissions
}

export type PropsWithUser<P = {}> = { user?: ApplicationUser } & P
export type AuthProps<P = {}> = PropsWithUser<P> | { authError: AuthError }

export interface AuthError {
  status?: string
  message: string
}

export type GetServerSidePropsContextWithUser = GetServerSidePropsContext & { req: { user?: ApplicationUser } }
export type Callback<P> = (context: GetServerSidePropsContextWithUser) => Promise<GetServerSidePropsResult<P>>

export interface AuthContextProps {
  user?: ApplicationUser
}
