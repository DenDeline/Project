
import { Button, Theme } from '@mui/material'

import Header from './Header'
import SignInUserBadge from './SignInUserBadge'

import { styled } from '@mui/material/styles'

import {useCallback} from 'react'
import {useRouter} from 'next/router'

const PREFIX = 'Layout'

const classes = {
  app: `${PREFIX}-app`
}

// TODO jss-to-styled codemod: The Fragment root was replaced by div. Change the tag if needed.
const Root = styled('div')((
  {
    theme
  }
) => ({
  [`& .${classes.app}`]: {
    minHeight: '100vh'
  }
}))

interface LayoutProps {
  user?: {
    username: string
    name?: string
    role?: string
  },
  title: string
}

const Layout: React.FC<LayoutProps> = ({user, title, children}) => {

  const router = useRouter()

  const handleLoginClick = useCallback(async () => {
    await router.push('/sign-in')
  }, [router])

  return (
    (<Root>
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
    </Root>)
  )
}


export default Layout
