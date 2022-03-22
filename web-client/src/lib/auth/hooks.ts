import { Roles, Permissions } from '@sentaku/constants'
import { ApplicationUser } from 'models/user'
import React from 'react'

export interface AuthContextProps {
  user?: ApplicationUser
}

export const AuthContext = React.createContext<AuthContextProps>({
  user: undefined
})

export const useUser = (): ApplicationUser | undefined => {
  const authContext = React.useContext(AuthContext)
  return authContext.user
}

export const useRoles = (): Roles[] => {
  const user = useUser()
  return React.useMemo(() => user === undefined ? [Roles.Unauthorized] : user.roles, [user])
}

export const usePermissions = (): Permissions => {
  const user = useUser()
  return React.useMemo(() => user?.permissions ?? Permissions.None, [user?.permissions])
}
