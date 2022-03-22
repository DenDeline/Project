
import { Button, Theme } from '@mui/material'

import Header from './Header'
import SignInUserBadge from './SignInUserBadge'

import { styled } from '@mui/material/styles'

import {useCallback} from 'react'
import {useRouter} from 'next/router'
import { useUser } from '@sentaku/lib'

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
  title: string
}

const Layout: React.FC<LayoutProps> = ({title, children}) => {
  const user = useUser()
  const router = useRouter()

  const handleLoginClick = useCallback(async () => {
    await router.push('/sign-in')
  }, [router])

  return (
    (<Root>
      <div className={classes.app}>
        <Header position={'static'} user={user}>
          {user
            ? <SignInUserBadge user={user} />
            : <Button color={'inherit'} onClick={handleLoginClick}>Login</Button>
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
