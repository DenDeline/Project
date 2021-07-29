import React, {useCallback, useEffect, useState} from "react";
import {DataGrid, GridCellParams, GridColDef} from '@material-ui/data-grid';
import {Button, Chip, Container, createStyles, Grid, makeStyles, Paper, Typography} from "@material-ui/core";
import Navbar from "../../components/Navbar";
import CheckIcon from '@material-ui/icons/Check';
import ClearIcon from '@material-ui/icons/Clear';
import ConfigureUserDialog from "../../components/ConfigureUserDialog";
import axios from "axios"
import {GetServerSideProps} from "next";

const useStyles = makeStyles(theme => 
    createStyles({
        title: {
            padding: theme.spacing(2),
            backgroundColor: 'rgba(0, 0, 0, .03)',
            borderBottom: '1px solid rgba(0, 0, 0, .125)',
        },
        content: {
          padding: theme.spacing(2)  
        }
    })
);

enum AppRoles {
    Administrator ='Administrator', 
    LeadManager = 'Lead Manager', 
    RepresentativeAuthority = 'Representative Authority', 
    Authority = 'Authority'
}

const getAvailableRoles = (currentUserRole: AppRoles): AppRoles[] => {
    switch (currentUserRole){
        case AppRoles.Administrator:
            return [ AppRoles.LeadManager, AppRoles.RepresentativeAuthority, AppRoles.Authority];
        case AppRoles.LeadManager:
            return [ AppRoles.RepresentativeAuthority, AppRoles.Authority];
        case AppRoles.RepresentativeAuthority:
            return [];
        case AppRoles.Authority:
            return [];
        default:
            return [];
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

interface RolesPanelProps {
    backendApi: string
}

export const getServerSideProps: GetServerSideProps<RolesPanelProps> = async (context) => {
    return {
        props: {
            backendApi: process.env.BACKEND_API_URL ?? ""
        }
    }
}

const RolePanel: React.FC<RolesPanelProps> = (props) => {
    const classes = useStyles();
    
    const [rows, setRows] = useState<User[]>([]);
    const [loading, setLoading] = useState<boolean>(false);
    
    useEffect(() => {
        let active = true;
        
        (async () => {
            setLoading(true);
            
            const usersResult = await axios.get<UserReadModel[]>( props.backendApi + "/users");
            const usersWithRoles = await Promise.all(usersResult.data.map(async (userResult: UserReadModel): Promise<User> => {
                const userRoles = await axios.get<string[]>(`${props.backendApi}/users/${userResult.username}/roles`);
                return {
                    ...userResult,
                    roles: userRoles.data
                };
            }));
            
            if (!active){
                return;
            }
            
            setRows(usersWithRoles);
            setLoading(false);
        })();
        
        return () => {
            active = false;
        }
    }, []);
    
    const [currentUser, ] = useState(rows[0]);
    const [permissions, ] = useState({
        editProfile: false,
        approvingDocuments: true,
        availableRoles: getAvailableRoles(AppRoles.LeadManager)
    });
    
    const [selectedUser, setSelectedUser] = useState(undefined);
    
    const [open, setOpen] = useState(false);
    
    const handleClickOpen = (user: any) => {
        setSelectedUser(user);
        setOpen(true);
    };
    
    const handleClose = () => {
        setOpen(false);
        setSelectedUser(undefined);
    };
    
    const handleSaveUser = useCallback((user: any) => {
        setRows(rows.filter(_ => _.id !== user.id).concat(user));
    }, [rows]);
    
    const columns: GridColDef[] = [
        { field: 'id', headerName: 'ID', disableReorder: true },
        { field: 'username', headerName: 'Username', flex: 1 },
        { field: 'fullname', headerName: 'Full Name', flex: 1, renderCell: (params: GridCellParams) => `${params.row.name} ${params.row.surname}` },
        { field: 'roles', headerName: 'Roles', flex: 1, renderCell: (params: GridCellParams) => ( 
            <Grid container spacing={1}>
                {
                    (params.value as string[]).map((item, index) => (
                        <Grid item key={index}>
                            <Chip label={item} color={'primary'}/>  
                        </Grid>
                    ))
                    
                }
            </Grid>
            ) },
        { field: 'verified', flex: 1, headerName: 'Documents approved', renderCell: params => (params.value ? <CheckIcon color={'primary'}/> : <ClearIcon color={'primary'}/>)},
        { field: 'actions', flex: 1, headerName: 'Actions', renderCell: (params: GridCellParams) => (
            <Grid container>
                <Grid item>
                    <Button onClick={() => handleClickOpen(params.row)}>Edit</Button>
                </Grid>
            </Grid>
            )}
    ];
    
    return (
        <>
            <Container maxWidth={'xl'}>
                <Typography component={'div'}>
                    <Navbar/>
                    <Paper variant={'outlined'}>
                        <Grid container direction={'column'}>
                            <Grid item className={classes.title}>
                                <Typography variant={'h5'}>
                                    Configure roles
                                </Typography>
                            </Grid>
                            <Grid item container direction={'column'} className={classes.content}>
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
        </>
    );
}



export default RolePanel;