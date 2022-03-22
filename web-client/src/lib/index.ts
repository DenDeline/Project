export type {
  AuthConfig,
  Callback,
  AuthProps,
  PropsWithUser,
  GetServerSidePropsContextWithUser,
  AuthError,
} from './auth'

export { withAuth, usePermissions, useUser, AuthContext } from './auth'

export { apiAxios, authAxios, fetcher } from './defaults'
