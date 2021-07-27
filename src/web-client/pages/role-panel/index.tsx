import {GetServerSideProps} from "next";
import React, {useCallback, useState} from "react";
import {DataGrid, GridCellParams, GridColDef} from '@material-ui/data-grid';
import Link from 'next/link'
import {
    Button,
    Card,
    Chip,
    Container,
    createStyles,
    Grid,
    Link as MaterialLink,
    makeStyles,
    Paper, Select,
    Typography
} from "@material-ui/core";
import Navbar from "../../components/Navbar";
import CheckIcon from '@material-ui/icons/Check';
import ClearIcon from '@material-ui/icons/Clear';
import ConfigureUserDialog from "../../components/ConfigureUserDialog";

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

const RolePanel: React.FC = (props) => {
    
    const classes = useStyles();
    const [users, setUsers] = useState(
        [
            {id : 1, username: 'admin', fullname: 'Rostislav Statko', verified: true, roles: ['Administrator']},
            {id : 2, username: 'DenDeline', fullname: 'Rostislav Statko', verified: false, roles: []},
        ]
    );
    
    const [availableRoles, setAvailableRoles] = useState([
        'Representative Authority', 'Authority', 'Administrator'
    ]);
    
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
        setUsers(users.filter(_ => _.id !== user.id).concat(user));
    }, [users]);
    
    const columns: GridColDef[] = [
        { field: 'id', headerName: 'ID', disableReorder: true },
        { field: 'username', headerName: 'Username', flex: 1 },
        { field: 'fullname', headerName: 'Full Name', flex: 1 },
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
                                    rows={users}
                                    columns={columns}
                                    pagination
                                    pageSize={30}
                                    rowCount={10}
                                />
                            </Grid>
                        </Grid>
                    </Paper>
                    <ConfigureUserDialog 
                        open={open} 
                        user={selectedUser}
                        availableRoles={availableRoles}
                        onSave={handleSaveUser}
                        onClose={handleClose}
                    />
                </Typography>
            </Container>
        </>
    );
}



export default RolePanel;