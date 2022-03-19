import React, {useCallback, useEffect, useState} from 'react'
import {
  Button, Checkbox, Chip, CircularProgress, createStyles,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle, FormControl, FormControlLabel, FormGroup, FormLabel, Grid, Input, makeStyles, Menu, MenuItem, Paper,
  TextField, Theme
} from '@material-ui/core'
import Link from 'next/link'
import {Add} from '@material-ui/icons'


interface ConfigureUserDialogProps {
  open: boolean,
  selectedUser?: {
    id: number,
    username: string,
    name: string,
    surname: string,
    verified: boolean
    roles: string[]
  },
  permissions: {
    editProfile: boolean,
    approvingDocuments: boolean,
    availableRoles: string[]
  }
  onSave: (user: any) => void
  onClose: () => void
}

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    root: {
      display: 'flex',
      flexWrap: 'wrap',
      listStyle: 'none',
      padding: theme.spacing(0.5),
      margin: 0,
    },
    chip: {
      margin: theme.spacing(0.5),
    },
  }),
)

const ConfigureUserDialog: React.FC<ConfigureUserDialogProps> = (props) => {
  const classes = useStyles()

  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null)

  const handleMenuClose = useCallback(() => {
    setAnchorEl(null)
  }, [setAnchorEl])

  const handleMenuClick = useCallback((event: React.MouseEvent<HTMLDivElement>) => {
    setAnchorEl(event.currentTarget)
  }, [])

  const [isUserChanged, setIsUserChanged] = useState<boolean>(false)

  const [userRoles, setUserRoles] = useState<string[]>([])

  const handleAppendRole = useCallback((role: string) => {
    setUserRoles(userRoles.concat(role))
    setIsUserChanged(true)
  }, [userRoles])

  const handleDeleteRole = useCallback((role: string) => {
    setUserRoles(userRoles.filter(_ => _ !== role))
    setIsUserChanged(true)
  }, [userRoles])

  const [userVerified, setUserVerified] = useState<boolean>(false)

  const handleUserVerification = useCallback((verified: boolean) => {
    setUserVerified(verified)
    if (!verified) {
      setUserRoles([])
    }
    setIsUserChanged(true)
  }, [])

  const [remindedRoles, setRemindedRoles] = useState<string[]>([])

  useEffect(() => {
    setRemindedRoles(props.permissions.availableRoles.filter(_ => !userRoles.includes(_)))
  }, [props.permissions.availableRoles, userRoles])

  useEffect(() => {
    if (props.open) {
      setUserRoles(props.selectedUser?.roles.slice() ?? [])
      setUserVerified(props.selectedUser?.verified ?? false)
    }
  }, [props.open])

  return (
    <Dialog
      fullWidth={true}
      maxWidth={'sm'}
      open={props.open}
      aria-labelledby={'dialog-title'}
      onClose={props.onClose}
    >
      <DialogTitle id={'dialog-title'}>
        Configure user: {props.selectedUser?.username}
      </DialogTitle>
      <DialogContent>
        <DialogContentText>
          Manage user account
        </DialogContentText>
        <Grid
          container
          direction={'column'}
          spacing={2}
        >
          <Grid item>
            <TextField
              label={'Username'}
              value={props.selectedUser?.username}
              disabled
            />
          </Grid>
          <Grid item>
            <TextField
              label={'Name'}
              value={props.selectedUser?.name}
              disabled
            />
          </Grid>
          <Grid item>
            <TextField
              label={'Name'}
              value={props.selectedUser?.surname}
              disabled
            />
          </Grid>
          <Grid item>
            <DialogContentText>
              Only approved users have access for our resource <br/>
              Links: <Link href={'/'}>Images</Link>
            </DialogContentText>
            <FormControlLabel
              control={<Checkbox checked={userVerified}/>}
              onClick={() => handleUserVerification(!userVerified)}
              label={'Documents approved'}
              disabled={!props.permissions.approvingDocuments}
            />
          </Grid>
          {props.permissions.availableRoles.length > 0 && userVerified && (
            <Grid item>
              <DialogContentText>
                User roles:
              </DialogContentText>
              <Paper component={'ul'} className={classes.root} variant={'outlined'}>
                {userRoles.map((userRole, index) => (
                    <li key={index}>
                      {props.permissions.availableRoles.some(_ => _ === userRole)
                        ? (
                          <Chip
                            label={userRole}
                            color={'primary'}
                            onDelete={() => {
                              handleDeleteRole(userRole)
                            }}
                            className={classes.chip}
                          />
                        )
                        : (
                          <Chip
                            label={userRole}
                            color={'primary'}
                            className={classes.chip}
                          />
                        )}
                    </li>
                  ))
                }
                {remindedRoles.length > 0 && (
                  <li key={userRoles.length}>
                    <Chip
                      aria-controls="user-roles-menu"
                      icon={<Add fontSize={'small'}/>}
                      label={'Add role'}
                      color={'default'}
                      onClick={handleMenuClick}
                      className={classes.chip}/>
                    <Menu
                      id="user-roles-menu"
                      anchorEl={anchorEl}
                      keepMounted
                      open={Boolean(anchorEl)}
                      onClose={handleMenuClose}
                    >
                      {
                        remindedRoles.length > 0 ?
                          remindedRoles.map(role => (
                            <MenuItem key={role} onClick={() => {
                              handleAppendRole(role)
                              handleMenuClose()
                            }}
                            >
                              {
                                role
                              }
                            </MenuItem>)) :
                          (
                            <MenuItem onClick={handleMenuClose}>None</MenuItem>
                          )
                      }
                    </Menu>
                  </li>
                  )
                }
              </Paper>
            </Grid>
            )
          }
        </Grid>
      </DialogContent>
      <DialogActions>
        <Button onClick={props.onClose} color={'primary'}>
          Close
        </Button>
        <Button onClick={() => {
          if (isUserChanged) {
            props.onSave({
              id: props.selectedUser?.id,
              username: props.selectedUser?.username,
              verified: userVerified,
              roles: userRoles
            })
          }
          props.onClose()
        }} color={'primary'} variant={'contained'}>
          Save
        </Button>
      </DialogActions>
    </Dialog>
  )
}

export default ConfigureUserDialog
