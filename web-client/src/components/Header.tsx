import { AppBar, Box, Drawer, IconButton, List, ListItem, Theme, Toolbar, Typography } from '@mui/material'
import { styled } from '@mui/material/styles'

import MenuIcon from '@mui/icons-material/Menu'
import { useCallback, useRef, useState } from 'react'

import { Link } from '@sentaku/components'
export interface HeaderProps {
  position: 'static' | 'sticky',
}

const Header: React.FC<HeaderProps> = ({position, children}) => {
  const [isNavigationMenuOpen, setIsNavigationMenuOpen] = useState(false)

  const handleNavigationMenuOpen = useCallback(() => {
    setIsNavigationMenuOpen(true)
  }, [])

  const handleNavigationMenuClose = useCallback(() => {
    setIsNavigationMenuOpen(false)
  } ,[])

  return (
    <Box sx={{ flexGrow: 1 }}>
      <AppBar position={position} color={'inherit'} >
        <Toolbar>
          <IconButton
            size={'large'}
            edge={'start'}
            aria-label={'menu'}
            onClick={handleNavigationMenuOpen}
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
            <ListItem component={Link} href={'/role-panel'}>
              Role panel
            </ListItem>
          </List>
        </Box>
      </Drawer>
    </Box>
  )
}

export default Header
