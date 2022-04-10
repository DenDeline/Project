import {
  Autocomplete,
  Button,
  Checkbox,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  FormControlLabel,
  Stack,
  TextField,
} from '@mui/material'

import {
  LoadingButton
} from '@mui/lab'

import Link from 'next/link'

import { Roles, RoleNames } from '@sentaku/constants'
import { ApplicationUser } from 'models/user'
import { useCallback, useState } from 'react'

interface ConfigureUserDialogProps {
  open: boolean,
  userId: ApplicationUser['id']
  roles: ApplicationUser['roles']
  username: ApplicationUser['username']
  name: ApplicationUser['name']
  surname: ApplicationUser['surname']
  verified: ApplicationUser['verified']
  onSave: (username: ApplicationUser['username']) => Promise<void>
  onClose: () => void,
  onExited: () => void,
  onRolesChange: (newRoles: Roles[]) => void,
  onVerificationChange: (verified: boolean) => void,
}

const ConfigureUserDialog: React.FC<ConfigureUserDialogProps> = ({
  onClose,
  onExited,
  onSave,
  open,
  userId,
  name,
  surname,
  username,
  roles,
  verified,
  onRolesChange,
  onVerificationChange
}) => {

  const [loading, setLoading] = useState(false)

  const handleUserSave = useCallback(() => {
    let active = true;

    (async () => {
      setLoading(true)
      await onSave(username)

      if (!active) {
        return
      }

      setLoading(false)
      onClose()
    })()

    return () => {
      active = false
    }
  }, [onClose, onSave, username])

  return (
    <Dialog
      fullWidth={true}
      maxWidth={'sm'}
      open={open}
      aria-labelledby={'dialog-title'}
      onClose={onClose}
      TransitionProps={{
        onExited: onExited
      }}
    >
      <DialogTitle id={'dialog-title'}>
        Configure user: {username}
      </DialogTitle>
      <DialogContent>
        <DialogContentText>
          Manage user account
        </DialogContentText>
        <Stack sx={{ mb: t => t.spacing(1) }}>
          <TextField
            label={'Username'}
            value={username}
            margin={'normal'}
            disabled
          />
          <TextField
              label={'Name'}
              value={name}
              margin={'normal'}
              disabled
            />
          <TextField
            label={'Name'}
            value={surname}
            margin={'normal'}
            disabled
          />
        </Stack>
        <DialogContentText>
          Only approved users have access for our resource <br/>
          Links: <Link href={'/'}>Images</Link>
        </DialogContentText>
        <FormControlLabel
          control={<Checkbox checked={verified} onClick={() => onVerificationChange(!verified)} />}
          label={'Documents approved'}
        />
        <Autocomplete
          multiple
          id={'user-roles'}
          options={[ Roles.Authority, Roles.LeadManager, Roles.RepresentativeAuthority ]}
          value={roles}
          getOptionLabel={option => RoleNames[option]}
          onChange={(e, v) => onRolesChange(v)}
          filterSelectedOptions
          disabled={!verified}
          ChipProps={{ color: 'secondary' }}
          renderInput={(params) => (
            <TextField
              {...params}
              margin={'normal'}
              label={'User roles'}
              placeholder={'User roles'}
            />
          )}
        />
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} color={'primary'}>
          Close
        </Button>
        <LoadingButton
          loading={loading}
          onClick={handleUserSave}
          color={'primary'}
          variant={'contained'}
          disableElevation
        >
          Save
        </LoadingButton>
      </DialogActions>
    </Dialog>
  )
}

export default ConfigureUserDialog
