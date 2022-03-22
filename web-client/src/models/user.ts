import { Permissions } from '@sentaku/constants'
export interface ApplicationUser {
  id: string
  username: string,
  name?: string,
  surname?: string,
  verified: boolean,
  roles: string[],
  permissions: Permissions
}
