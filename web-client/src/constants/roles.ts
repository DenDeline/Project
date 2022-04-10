export enum Roles {
  Administrator = 'Administrator',
  LeadManager = 'LeadManager',
  RepresentativeAuthority = 'RepresentativeAuthority',
  Authority = 'Authority',
  Unauthorized = '__unauthorized'
}

export const RoleNames = {
  [Roles.Administrator]: 'Administrator',
  [Roles.Authority]: 'Authority',
  [Roles.LeadManager]: 'Lead Manager',
  [Roles.RepresentativeAuthority]: 'Representative Authority',
  [Roles.Unauthorized]: 'Unauthorized'
}
