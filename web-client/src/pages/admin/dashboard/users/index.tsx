import {AuthProps, withAuth} from '@sentaku/lib'

import { Button, Chip, Container, Grid, Paper, Typography } from '@mui/material'
import {ConfigureUserDialog, Layout} from '@sentaku/components'
import {DataGrid, GridCellParams, GridColDef} from '@mui/x-data-grid'
import {useCallback, useEffect, useState} from 'react'

import CheckIcon from '@mui/icons-material/Check'
import ClearIcon from '@mui/icons-material/Clear'

import {GetServerSideProps, NextPage} from 'next'
import {Permissions, Roles, RoleNames} from '@sentaku/constants'

import axios from 'axios'
import { ApplicationUser } from 'models/user'

interface UserReadModel {
  id: string,
  username: string,
  name: string,
  surname: string,
  verified: boolean,
  createdAt: Date
}

export const getServerSideProps: GetServerSideProps = withAuth(async (context) => {
  return {
    props: {

    }
  }
}, {withRedirect: true, permissions: Permissions.Administrator })

const RolePanel: NextPage = () => {

  const [rows, setRows] = useState<Omit<ApplicationUser, 'permissions'>[]>([])
  const [isGridLoading, setIsGridLoading] = useState<boolean>(false)

  useEffect(() => {
    let active = true;

    (async () => {
      setIsGridLoading(true)
      const usersResult = await axios.get<UserReadModel[]>('/api/users')

      const usersWithRoles = await Promise.all(usersResult.data.map(async (userResult: UserReadModel): Promise<Omit<ApplicationUser, 'permissions'>> => {
        const userRoles = await axios.get<{ roles: string[] }>(`/api/users/${userResult.username}/roles`)
        return {
          roles: userRoles.data.roles as Roles[],
          ...userResult
        }
      }))

      if (!active) {
        return
      }

      setRows(usersWithRoles)
      setIsGridLoading(false)
    })()

    return () => {
      active = false
    }
  }, [])

  const RolesCell = useCallback((params: GridCellParams<Element, ApplicationUser>) => (
    <Grid container spacing={1}>
      {params.row.roles.map(role => (
        <Grid item key={role}>
          <Chip label={RoleNames[role]} color={'secondary'}/>
        </Grid>
      ))}
    </Grid>
  ), [])

  const ApprovedDocumentCell = useCallback((params: GridCellParams<Element, ApplicationUser>) =>
    params.value
      ? <CheckIcon color={'primary'}/>
      : <ClearIcon color={'primary'}/>
  , [])

  const EditAction = useCallback((params: GridCellParams<Element, ApplicationUser>) =>
      <Button onClick={() => handleConfigureUserButtonClick(params.row)}>Edit</Button>
  , [])

  const columns: GridColDef[] = [
    {field: 'id', headerName: 'ID', disableReorder: true},
    {field: 'username', headerName: 'Username', flex: 1},
    {field: 'name', headerName: 'Name', flex: 1},
    {field: 'surname', headerName: 'Surname', flex: 1},
    {field: 'createdAt', headerName: 'Created on', flex: 1},
    {field: 'roles', headerName: 'Roles', flex: 1, renderCell: RolesCell},
    {field: 'verified', flex: 1, headerName: 'Documents approved', renderCell: ApprovedDocumentCell},
    {field: 'actions', flex: 1, headerName: 'Actions', renderCell: EditAction}
  ]

  // User config dialog

  const [isConfigureUserDialogOpen, setIsConfigureUserDialogOpen] = useState(false)

  const [selectedUser, setSelectedUser] = useState<ApplicationUser | null>(null)

  const [selectedUserRoles, setSelectedUserRoles] = useState<Roles[]>([])
  const [selectedUserVerification, setSelectedUserVerification] = useState(false)

  const handleConfigureUserButtonClick = (user: ApplicationUser) => {
    setSelectedUserRoles(user.roles)
    setSelectedUserVerification(user.verified)
    setSelectedUser(user)
    setIsConfigureUserDialogOpen(true)
  }

  const handleConfigureUserDialogClose = () => {
    setIsConfigureUserDialogOpen(false)
  }

  const handleConfigureUserDialogExited = useCallback(() => {
    setSelectedUser(null)
    setSelectedUserRoles([])
    setSelectedUserVerification(false)
  }, [])

  const handleSaveUser = useCallback(async (username: ApplicationUser['name']) => {

    await axios.post(`/api/users/${username}/verify`, { verified: selectedUserVerification })

    if (selectedUserVerification) {
      await axios.post<{ roles: Roles[] }>(`/api/users/${username}/roles`, { roles: selectedUserRoles })
    }

    setRows(rows.map(user => user.username !== username ? user : { ...user, roles: selectedUserRoles, verified: selectedUserVerification }))

  }, [rows, selectedUserRoles, selectedUserVerification])

  return (
    <Layout
      title={'Users'}
    >
      <Container maxWidth={'xl'}>
          <Grid container direction={'column'} sx={{ mb: theme => theme.spacing(2) }}>
            <Grid item sx={{ p: theme => theme.spacing(2), backgroundColor: t => t.palette.grey[100] }}>
              <Typography variant={'h5'}>
                Configure users
              </Typography>
            </Grid>
          </Grid>
          <DataGrid
            autoHeight
            rows={rows}
            columns={columns}
            loading={isGridLoading}
            hideFooterSelectedRowCount={true}
          />
          <ConfigureUserDialog
            open={isConfigureUserDialogOpen}
            userId={selectedUser?.id ?? ''}
            name={selectedUser?.name ?? ''}
            surname={selectedUser?.surname ?? ''}
            roles={selectedUserRoles}
            verified={selectedUserVerification}
            username={selectedUser?.username ?? ''}
            onRolesChange={roles => setSelectedUserRoles(roles)}
            onVerificationChange={verified => setSelectedUserVerification(verified)}
            onExited={handleConfigureUserDialogExited}
            onSave={handleSaveUser}
            onClose={handleConfigureUserDialogClose}
          />
      </Container>
    </Layout>
  )
}


export default RolePanel
