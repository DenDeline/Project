import { AppBar, Theme, Toolbar, Typography } from '@mui/material'
import { styled } from '@mui/material/styles'

const PREFIX = 'Header'

const classes = {
  navbar: `${PREFIX}-navbar`,
  title: `${PREFIX}-title`
}

const StyledAppBar = styled(AppBar)(({theme}) => ({
  [`&.${classes.navbar}`]: {
    marginBottom: theme.spacing(2)
  },

  [`& .${classes.title}`]: {
    flexGrow: 1
  }
}))

export interface HeaderProps {
  position: 'static' | 'sticky',
}

const Header: React.FC<HeaderProps> = ({position, children}) => {

  return (
    <StyledAppBar
      position={position}
      color={'inherit'}
      className={classes.navbar}
    >
      <Toolbar>
        <Typography
          variant={'h3'}
          noWrap
          className={classes.title}
        >
          Vote
        </Typography>
        <div
          style={{marginLeft: 'auto'}}
        >
          {children}
        </div>

      </Toolbar>
    </StyledAppBar>
  )
}

export default Header
