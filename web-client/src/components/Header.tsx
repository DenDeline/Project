import { AppBar, Box, IconButton, Theme, Toolbar, Typography } from '@mui/material'
import { styled } from '@mui/material/styles'

import MenuIcon from '@mui/icons-material/Menu'
export interface HeaderProps {
  position: 'static' | 'sticky',
}

const Header: React.FC<HeaderProps> = ({position, children}) => {

  return (
    <Box sx={{ flexGrow: 1 }}>
      <AppBar position={position} >
        <Toolbar>
          <IconButton
            size="large"
            edge="start"
            color="inherit"
            aria-label="menu"
            sx={{ mr: 2 }}
          >
            <MenuIcon />
          </IconButton>
          <Typography
            variant={'h6'}
            component={'div'}
            sx={{ flexGrow: 1 }}
          >
            Vote
          </Typography>
          <Box>
            {children}
          </Box>
        </Toolbar>
      </AppBar>
    </Box>
  )
}

export default Header
