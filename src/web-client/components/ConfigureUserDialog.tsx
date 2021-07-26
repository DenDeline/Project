import React, {useCallback, useEffect, useState} from "react";
import {
    Button, Checkbox, Chip, CircularProgress, createStyles,
    Dialog,
    DialogActions,
    DialogContent,
    DialogContentText,
    DialogTitle, FormControl, FormControlLabel, FormGroup, FormLabel, Grid, Input, makeStyles, Menu, MenuItem, Paper,
    TextField, Theme
} from "@material-ui/core";
import Link from 'next/link'
import {Add} from "@material-ui/icons";


interface ConfigureUserDialogProps {
    open: boolean,
    user?: {
        id: number,
        username: string,
        fullname: string,
        verified: boolean
        roles: string[]
    },
    availableRoles: string[]
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
);

const ConfigureUserDialog: React.FC<ConfigureUserDialogProps> = (props) => {
    const classes = useStyles();
    
    const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);

    const handleMenuClose = useCallback(() => {
        setAnchorEl(null);
    }, [setAnchorEl]);

    const handleMenuClick = useCallback((event: React.MouseEvent<HTMLDivElement>) => {
        setAnchorEl(event.currentTarget);
    }, []);
    
    const [isUserChanged, setIsUserChanged] = useState<boolean>(false);
    
    const [userRoles, setUserRoles] = useState<string[]>( []);
    
    const handleAppendRole = useCallback((role: string) => {
        setUserRoles(userRoles.concat(role));
        setIsUserChanged(true);
    }, [userRoles])
    
    const handleDeleteRole = useCallback((role: string) => {
        setUserRoles(userRoles.filter(_ => _ !== role));
        setIsUserChanged(true);
    }, [userRoles]);
    
    const [userVerified, setUserVerified] = useState<boolean>(false);
    
    const handleUserVerification = useCallback((verified: boolean) => {
        setUserVerified(verified);
        if (!verified){
            setUserRoles([]);
        }
        setIsUserChanged(true);
    }, []);
    
    const [availableRolesForUser, setAvailableRolesForUser] = useState<string[]>([]);

    useEffect(() => {
        setUserRoles(props.user?.roles.slice() ?? []);
        setUserVerified(props.user?.verified ?? false);
    }, [props.user]);
    
    useEffect(() => {
        setAvailableRolesForUser(props.availableRoles.filter(_ => ! userRoles.includes(_)));
    }, [props.availableRoles, userRoles]);
    
    return (
        <Dialog fullWidth={true} maxWidth={'sm'} open={props.open} aria-labelledby="dialog-title" onClose={props.onClose}>
            <DialogTitle id={"dialog-title"}>Configure user: {props.user?.username}</DialogTitle>
            <DialogContent>
                <DialogContentText>
                    Manage user account
                </DialogContentText>
                <Grid container direction={'column'} spacing={2}>
                    <Grid item>
                        <TextField label={"Username"} value={props.user?.username} disabled/>
                    </Grid>
                    <Grid item>
                        <TextField label={"Full name"} value={props.user?.fullname} disabled/>
                    </Grid>
                    <Grid item>
                        <DialogContentText>
                            Only approved users have access for our resource <br/>
                            Links: <Link href={'/'}>Images</Link>
                        </DialogContentText>
                        <FormControlLabel control={<Checkbox checked={userVerified}/>} onClick={() => {
                            handleUserVerification(!userVerified);
                        }} label={"Documents approved"}/>
                    </Grid>
                    {
                        userVerified && (
                            <Grid item>
                                <DialogContentText>
                                    User roles:
                                </DialogContentText>
                                <Paper component={'ul'} className={classes.root} variant={'outlined'}>
                                    {
                                        userRoles.map((userRole, index) => (
                                            <li key={index}>
                                                <Chip
                                                    label={userRole}
                                                    color={'primary'}
                                                    onDelete={() => {
                                                        handleDeleteRole(userRole);
                                                    }}
                                                    className={classes.chip}/>
                                            </li>
                                        ))
                                    }
                                    <li key={userRoles.length}>
                                        <Chip
                                            aria-controls="user-roles-menu"
                                            icon={<Add fontSize={'small'}/>}
                                            label={"Add role"}
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
                                                availableRolesForUser.length > 0 ?
                                                    availableRolesForUser.map(role => (
                                                        <MenuItem onClick={() => {
                                                            handleAppendRole(role);
                                                            handleMenuClose();}}
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
                    if (isUserChanged){
                        props.onSave({
                            id: props.user?.id,
                            username: props.user?.username,
                            fullname: props.user?.fullname,
                            verified: userVerified,
                            roles: userRoles
                        });
                    }
                    props.onClose();
                }} color={'primary'} variant={'contained'}>
                    Save
                </Button>
            </DialogActions>
        </Dialog>
    );
}

export default ConfigureUserDialog;