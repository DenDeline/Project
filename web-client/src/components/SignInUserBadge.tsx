import { Avatar, Box, Grid, IconButton, Link as MaterialLink, Menu, MenuItem, Theme, Tooltip, Typography } from '@mui/material'

import Image from 'next/image'

import {AccountCircle} from '@mui/icons-material'
import { Link } from '.'

import { styled } from '@mui/material/styles'
import { useCallback, useRef, useState } from 'react'

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

  const [isUserMenuOpen, setIsUserMenuOpen] = useState(false)
  const menuAnchorElRef = useRef(null)

  const handleUserMenuOpen = useCallback(() => {
    setIsUserMenuOpen(true)
  }, [])

  const handleUserMenuClose = useCallback(() => {
    setIsUserMenuOpen(false)
  }, [])

  return (
    <Box sx={{ flexGrow: 0 }}>
      <Tooltip title={'Open user menu'}>
        <IconButton
          ref={menuAnchorElRef}
          sx={{ p: 0 }}
          onClick={handleUserMenuOpen}
        >
          <Avatar src={'http://localhost:3000/api/user/profileImage'} />
        </IconButton>
      </Tooltip>
      <Menu
        sx={{ mt: '45px' }}
        open={isUserMenuOpen}
        anchorEl={menuAnchorElRef.current}
        anchorOrigin={{
          vertical: 'top',
          horizontal: 'right',
        }}
        keepMounted
        transformOrigin={{
          vertical: 'top',
          horizontal: 'right',
        }}
        onClose={handleUserMenuClose}
      >
        <MenuItem component={Link} href={'/settings/profile'}>Profile</MenuItem>
        <MenuItem component={Link} href={'/logout'}>Logout</MenuItem>
      </Menu>
    </Box>
  )
}

export default SignInUserBadge
