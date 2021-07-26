import {GetServerSideProps} from "next";
import React, {useState} from "react";
import {DataGrid, GridColDef} from '@material-ui/data-grid';
import Link from 'next/link'
import {Card, Container, createStyles, Grid, Link as MaterialLink, makeStyles, Paper, Typography} from "@material-ui/core";
import Navbar from "../../components/Navbar";
import CheckIcon from '@material-ui/icons/Check';
import ClearIcon from '@material-ui/icons/Clear';

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
    
    const columns: GridColDef[] = [
        { field: 'id', headerName: 'ID' },
        { field: 'username', headerName: 'Username', flex: 1 },
        { field: 'fullname', headerName: 'Full Name', flex: 1 },
        { field: 'hasApprovedDocs', flex: 1, headerName: 'Documents approved', renderCell: params => (params.value ? <CheckIcon color={'primary'}/> : <ClearIcon color={'primary'}/>)}
    ];
    
    const rows = [
        {id : 1, username: 'admin', fullname: 'Rostislav Statko', hasApprovedDocs: true},
        {id : 2, username: 'DenDeline', fullname: 'Rostislav Statko', hasApprovedDocs: false},
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
                                <Grid item>
                                    
                                </Grid>
                                <Grid item container >
                                    <Grid item sm={6} container direction={'column'}>
                                        <Grid item>
                                            <Typography variant={'h5'} gutterBottom>
                                                1) Select users
                                            </Typography>
                                        </Grid>
                                        <Grid item>
                                            <DataGrid
                                                checkboxSelection
                                                autoHeight
                                                rows={rows}
                                                columns={columns}
                                                pagination
                                                pageSize={30}
                                                rowCount={10}
                                            />
                                        </Grid>
                                    </Grid>
                                </Grid>
                            </Grid>
                        </Grid>
                    </Paper>
                </Typography>
            </Container>
        </>
    );
}



export default RolePanel;