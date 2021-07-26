import React from "react";
import {createStyles, Grid, Link as MaterialLink, makeStyles, Paper} from "@material-ui/core";
import Link from "next/link";

const useStyles = makeStyles(theme =>
    createStyles({
        navbar: {
            padding: theme.spacing(1.5),
            marginBottom: theme.spacing(2)
        }
    })
);

const Navbar: React.FC = () => {
    const classes = useStyles();
    return (
        <Paper variant={'outlined'} className={classes.navbar}>
            <Grid container spacing={3}>
                <Grid item>
                    <Link href={'/voting'} passHref={true}>
                        <MaterialLink>
                            Current Vote
                        </MaterialLink>
                    </Link>
                </Grid>
                <Grid item>
                    <Link href={'/'} passHref={true}>
                        <MaterialLink>
                            History
                        </MaterialLink>
                    </Link>
                </Grid>
                <Grid item>
                    <Link href={'/role-panel'} passHref={true}>
                        <MaterialLink>
                            Role panel
                        </MaterialLink>
                    </Link>
                </Grid>
            </Grid>
        </Paper>
    );
}

export default Navbar;