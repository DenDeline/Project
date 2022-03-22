import { AppBar, Box, Drawer, IconButton, List, ListItem, Theme, Toolbar, Typography } from '@mui/material'
import { styled } from '@mui/material/styles'

import MenuIcon from '@mui/icons-material/Menu'
import { useCallback, useRef, useState } from 'react'

import { Link } from '@sentaku/components'
import { ApplicationUser } from 'models/user'
import { Permissions } from '@sentaku/constants'
import { useUser } from '@sentaku/lib'

export interface HeaderProps {
  position: 'static' | 'sticky',
  user?: ApplicationUser
}

const Header: React.FC<HeaderProps> = ({position, children, user}) => {

  const [isNavigationMenuOpen, setIsNavigationMenuOpen] = useState(false)

  const handleNavigationMenuOpen = useCallback(() => {
    setIsNavigationMenuOpen(true)
  }, [])

  const handleNavigationMenuClose = useCallback(() => {
    setIsNavigationMenuOpen(false)
  } ,[])

  return (
    <Box sx={{ flexGrow: 1, mb: theme => theme.spacing(2) }}>
      <AppBar position={position} color={'inherit'} >
        <Toolbar>
          {user && (
            <IconButton
              size={'large'}
              edge={'start'}
              aria-label={'menu'}
              onClick={handleNavigationMenuOpen}
              sx={{ mr: 2 }}
            >
              <MenuIcon />
            </IconButton>
          )}
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
      {user && (
        <Drawer
          open={isNavigationMenuOpen}
          anchor={'left'}
          onClose={handleNavigationMenuClose}
        >
          <Box sx={{ width: 320 }}>
            <List>
              <ListItem component={Link} href={'/voting'}>
                Current vote
              </ListItem>
              {(user.permissions & Permissions.Administrator) === Permissions.Administrator && (
                <ListItem component={Link} href={'/role-panel'}>
                  Admin panel
                </ListItem>
              )}
            </List>
          </Box>
        </Drawer>
      )}
    </Box>
  )
}

export default Header
