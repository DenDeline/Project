
import { Button, Theme } from '@mui/material'

import Header from './Header'
import SignInUserBadge from './SignInUserBadge'

import createStyles from '@mui/styles/createStyles'
import makeStyles from '@mui/styles/makeStyles'
import {useCallback} from 'react'
import {useRouter} from 'next/router'


const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    app: {
      minHeight: '100vh'
    }
  })
)

interface LayoutProps {
  user?: {
    username: string
    name?: string
    role?: string
  },
  title: string
}

const Layout: React.FC<LayoutProps> = ({user, title, children}) => {
  const classes = useStyles()
  const router = useRouter()

  const handleLoginClick = useCallback(async () => {
    await router.push('/sign-in')
  }, [router])

  return (
    <>
      <div className={classes.app}>
        <Header position={'static'}>
          {user
            ? <SignInUserBadge user={user}/>
            : <Button onClick={handleLoginClick}>Login</Button>
          }
        </Header>
        <div>
          {children}
        </div>
      </div>
    </>
  )
}


export default Layout
