import {AuthProps, withAuth} from '@sentaku/lib'

import { Button, Chip, Container, Grid, Paper, Typography } from '@mui/material'
import {ConfigureUserDialog, Layout} from '@sentaku/components'
import {DataGrid, GridCellParams, GridColDef} from '@mui/x-data-grid'
import {useCallback, useEffect, useState} from 'react'

import CheckIcon from '@mui/icons-material/Check'
import ClearIcon from '@mui/icons-material/Clear'

import {GetServerSideProps} from 'next'
import {Permissions} from '@sentaku/constants'

import axios from 'axios'

import { styled } from '@mui/material/styles'

const PREFIX = 'RolePanel'

const classes = {
  title: `${PREFIX}-title`,
  content: `${PREFIX}-content`
}

const StyledLayout = styled(Layout)((
  {
    theme
  }
) => ({
  [`& .${classes.title}`]: {
    padding: theme.spacing(2),
    backgroundColor: 'rgba(0, 0, 0, .03)',
    borderBottom: '1px solid rgba(0, 0, 0, .125)',
  },

  [`& .${classes.content}`]: {
    padding: theme.spacing(2)
  }
}))

enum AppRoles {
  Administrator = 'Administrator',
  LeadManager = 'Lead Manager',
  RepresentativeAuthority = 'Representative Authority',
  Authority = 'Authority'
}

const getAvailableRoles = (currentUserRole: AppRoles): AppRoles[] => {
  switch (currentUserRole) {
    case AppRoles.Administrator:
      return [AppRoles.LeadManager, AppRoles.RepresentativeAuthority, AppRoles.Authority]
    case AppRoles.LeadManager:
      return [AppRoles.RepresentativeAuthority, AppRoles.Authority]
    case AppRoles.RepresentativeAuthority:
      return []
    case AppRoles.Authority:
      return []
    default:
      return []
  }
}

interface User {
  id: number,
  username: string,
  name: string,
  surname: string,
  verified: string,
  roles?: string[]
}

interface UserReadModel {
  id: number,
  username: string,
  name: string,
  surname: string,
  verified: string,
}

export const getServerSideProps: GetServerSideProps = withAuth(async (context) => {
  return {
    props: {

    }
  }
}, {withRedirect: true, permissions: Permissions.ManageUserRoles })

const RolePanel: React.FC<AuthProps> = ({data, error}) => {


  const [rows, setRows] = useState<User[]>([])
  const [loading, setLoading] = useState<boolean>(false)

  useEffect(() => {
    let active = true;

    (async () => {
      setLoading(true)
      const usersResult = await axios.get<UserReadModel[]>('/api/users')

      const usersWithRoles = await Promise.all(usersResult.data.map(async (userResult: UserReadModel): Promise<User> => {
        const userRoles = await axios.get<{ roles: string[] }>(`api/users/${userResult.username}/roles`)
        return {
          ...userResult,
          roles: userRoles.data.roles
        }
      }))

      if (!active) {
        return
      }

      setRows(usersWithRoles)
      setLoading(false)
    })()

    return () => {
      active = false
    }
  }, [])

  const [permissions,] = useState({
    editProfile: false,
    approvingDocuments: true,
    availableRoles: getAvailableRoles(AppRoles.Administrator)
  })

  const [selectedUser, setSelectedUser] = useState(undefined)

  const [open, setOpen] = useState(false)

  const handleClickOpen = (user: any) => {
    setSelectedUser(user)
    setOpen(true)
  }

  const handleClose = () => {
    setOpen(false)
    setSelectedUser(undefined)
  }

  const handleSaveUser = useCallback((user: any) => {
    setRows(rows.filter(_ => _.id !== user.id).concat(user))
  }, [rows])

  const ApprovedDocumentCell = useCallback(params =>
      params.value
        ? <CheckIcon color={'primary'}/>
        : <ClearIcon color={'primary'}/>,
    [])

  const EditAction = useCallback((params: GridCellParams) =>
      <Button onClick={() => handleClickOpen(params.row)}>Edit</Button>
    , [])

  const columns: GridColDef[] = [
    {field: 'id', headerName: 'ID', disableReorder: true},
    {field: 'username', headerName: 'Username', flex: 1},
    {
      field: 'fullname',
      headerName: 'Full Name',
      flex: 1,
      renderCell: (params: GridCellParams) => `${params.row.name} ${params.row.surname}`
    },
    {
      field: 'roles', headerName: 'Roles', flex: 1, renderCell: function RolesGrid(params: GridCellParams) {
        return (
          <Grid container spacing={1}>
            {
              (params.value as string[])?.map((item, index) => (
                <Grid item key={index}>
                  <Chip label={item} color={'primary'}/>
                </Grid>
              ))

            }
          </Grid>
        )
      }
    },
    {field: 'verified', flex: 1, headerName: 'Documents approved', renderCell: ApprovedDocumentCell},
    {field: 'actions', flex: 1, headerName: 'Actions', renderCell: EditAction}
  ]

  return (
    <Layout
      title={'Role panel'}
      user={data?.user}
    >
      <Container maxWidth={'xl'}>
        <Typography component={'div'}>
          <Paper variant={'outlined'}>
            <Grid container direction={'column'}>
              <Grid item sx={{ p: theme => theme.spacing(2), backgroundColor: 'rgba(0, 0, 0, .03)', borderBottom: '1px solid rgba(0, 0, 0, .125)' }}>
                <Typography variant={'h5'}>
                  Configure roles
                </Typography>
              </Grid>
              <Grid item container direction={'column'} sx={{ p: theme => theme.spacing(2) }}>
                <DataGrid
                  autoHeight
                  rows={rows}
                  columns={columns}
                  loading={loading}
                  hideFooterSelectedRowCount={true}
                />
              </Grid>
            </Grid>
          </Paper>
          <ConfigureUserDialog
            open={open}
            selectedUser={selectedUser}
            permissions={permissions}
            onSave={handleSaveUser}
            onClose={handleClose}
          />
        </Typography>
      </Container>
    </Layout>
  )
}


export default RolePanel
