import React, {useState} from "react";
import Link from "next/link";
import {
    Link as MaterialLink,
    AppBar,
    Button,
    Container,
    createStyles,
    Grid,
    Icon,
    makeStyles,
    Theme,
    Toolbar,
    Typography
} from "@material-ui/core";
import { AccountCircle } from "@material-ui/icons";

interface LayoutProps {
    
}

const useStyles = makeStyles((theme: Theme) => 
    createStyles({
        app: {
            minHeight: '100vh'
        },
        navbar: {
            marginBottom: theme.spacing(2)
        },
        userAvatar: {
            margin: 'auto',
            height: '100%'
        },
        title: {
            flexGrow: 1
        }   
    })
);

interface User {
    name: string,
    role: string
}

const Layout: React.FC<LayoutProps> = (props) => {
    const classes = useStyles();
    const [user, setUser] = useState<User | undefined>({
        name: 'DenDeline',
        role: 'Representative Authority'
    });
    
    return(
        <div className={classes.app}>
            <AppBar position={'static'} color={'inherit'} className={classes.navbar}>
                <Toolbar>
                    <Typography variant={'h3'} noWrap className={classes.title}>
                        Vote
                    </Typography>
                    {user ? (
                        <div>
                            <Grid container spacing={1}>
                                <Grid item >
                                    <AccountCircle className={classes.userAvatar}/>
                                </Grid>
                                <Grid item xs={12} sm container direction={'column'} spacing={1}>
                                    <Grid item xs>
                                        <Typography gutterBottom variant={'caption'}>
                                            Hello, { user.name } ( { user.role } )
                                        </Typography>
                                        <Grid item>
                                            <Link href={`/${user.name}/profile`} passHref={true}>
                                                <MaterialLink>
                                                    My profile
                                                </MaterialLink>
                                            </Link>
                                        </Grid>
                                    </Grid>
                                </Grid>
                            </Grid>
                        </div>
                    ) : (
                        <div>
                            <Grid container spacing={1}>
                                <Grid item>
                                    <Link href={'/sign-in'} passHref={true}>
                                        <MaterialLink>
                                            Sign in
                                        </MaterialLink>
                                    </Link>
                                </Grid>
                                <Grid item>
                                    <span>/</span>
                                </Grid>
                                <Grid item>
                                    <Link href={'/'} passHref={true}>
                                        <MaterialLink>
                                            Sign up
                                        </MaterialLink>
                                    </Link>
                                </Grid>
                            </Grid>
                        </div>
                    )}
                </Toolbar>
            </AppBar>
            
            <div>
                {
                    props.children
                }
            </div>
        </div>
    );
}



export default Layout;