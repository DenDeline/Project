import { Permissions, Roles } from '@sentaku/constants'
export interface ApplicationUser {
  id: string
  username: string,
  name?: string,
  surname?: string,
  verified: boolean,
  roles: Roles[],
  permissions: Permissions
}
