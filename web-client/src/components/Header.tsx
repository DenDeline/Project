import {
  AppBar,
  Theme,
  Toolbar,
  Typography,
  createStyles,
  makeStyles
} from '@material-ui/core'

export interface HeaderProps {
  position: 'static' | 'sticky',
}

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    navbar: {
      marginBottom: theme.spacing(2)
    },
    title: {
      flexGrow: 1
    }
  })
)


const Header: React.FC<HeaderProps> = ({position, children}) => {
  const classes = useStyles()
  return (
    <AppBar
      position={position}
      color={'inherit'}
      className={classes.navbar}
    >
      <Toolbar>
        <Typography
          variant={'h3'}
          noWrap
          className={classes.title}
        >
          Vote
        </Typography>
        <div
          style={{marginLeft: 'auto'}}
        >
          {children}
        </div>

      </Toolbar>
    </AppBar>
  )
}

export default Header
