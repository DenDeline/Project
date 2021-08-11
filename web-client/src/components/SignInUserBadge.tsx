import React from "react";
import {createStyles, Grid, Link as MaterialLink, makeStyles, Theme, Typography} from "@material-ui/core";
import {AccountCircle} from "@material-ui/icons";
import Link from "next/link";


const useStyles = makeStyles((theme: Theme) =>
    createStyles({
        userAvatar: {
            margin: 'auto',
            height: '100%'
        }
    })
)

export interface SignInUserBadgeProps {
    user: {
        username: string
        name?: string
        role?: string
    }
}

const SignInUserBadge: React.FC<SignInUserBadgeProps> = ({user}) => {
    const classes = useStyles()
    return (
        <Grid container spacing={1}>
            <Grid item>
                <AccountCircle className={classes.userAvatar}/>
            </Grid>
            <Grid item sm>
                <Typography gutterBottom variant={'caption'}>
                    Hello, { user.name || user.username } { user.role && ( <>({ user.role })</>) }
                </Typography>
                <br/>
                <Link href={`/${user.username}/profile`} passHref={true}>
                    <MaterialLink>
                        My profile
                    </MaterialLink>
                </Link>
            </Grid>
        </Grid>
    )
}

export default SignInUserBadge;