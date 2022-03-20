import { Grid, Link as MaterialLink, Paper } from '@mui/material'

import Link from 'next/link'

import { styled } from '@mui/material/styles'

const PREFIX = 'Navbar'

const classes = {
  navbar: `${PREFIX}-navbar`
}

const StyledPaper = styled(Paper)((
  {
    theme
  }
) => ({
  [`&.${classes.navbar}`]: {
    padding: theme.spacing(1.5),
    marginBottom: theme.spacing(2)
  }
}))

const Navbar: React.FC = () => {

  return (
    <StyledPaper variant={'outlined'} className={classes.navbar}>
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
    </StyledPaper>
  )
}

export default Navbar
