import { Grid, Link as MaterialLink, Theme, Typography } from '@mui/material'

import {AccountCircle} from '@mui/icons-material'
import Link from 'next/link'

import { styled } from '@mui/material/styles'

const PREFIX = 'SignInUserBadge'

const classes = {
  userAvatar: `${PREFIX}-userAvatar`
}

const StyledGrid = styled(Grid)((
  {
    theme: Theme
  }
) => ({
  [`& .${classes.userAvatar}`]: {
    margin: 'auto',
    height: '100%'
  }
}))

export interface SignInUserBadgeProps {
  user: {
    username: string
    name?: string
    role?: string
  }
}

const SignInUserBadge: React.FC<SignInUserBadgeProps> = ({user}) => {

  return (
    <StyledGrid container spacing={1}>
      <Grid item>
        <AccountCircle className={classes.userAvatar}/>
      </Grid>
      <Grid item sm>
        <Typography gutterBottom variant={'caption'}>
          Hello, {user.name || user.username} {user.role && (<>({user.role})</>)}
        </Typography>
        <br/>
        <Link href={'/settings/profile'} passHref={true}>
          <MaterialLink>
            My profile
          </MaterialLink>
        </Link>
      </Grid>
    </StyledGrid>
  )
}

export default SignInUserBadge
