import { CacheProvider, EmotionCache } from '@emotion/react'
import { StyledEngineProvider, Theme, ThemeProvider } from '@mui/material/styles'

import {AppProps} from 'next/app'
import CssBaseline from '@mui/material/CssBaseline'
import Head from 'next/head'

import { createEmotionCache } from '@sentaku/utils'
import { lightTheme } from '@sentaku/styles/theme'
import { SWRConfig } from 'swr'
import { fetcher, PropsWithUser, AuthContext, AuthProps } from '@sentaku/lib'

interface MyAppProps extends AppProps<AuthProps> {
  emotionCache?: EmotionCache;
}

const clientSideEmotionCache = createEmotionCache()

export default function MyApp(props: MyAppProps) {
  const {Component, pageProps, emotionCache = clientSideEmotionCache} = props
  const { user } = pageProps as PropsWithUser

  return (
    <CacheProvider value={emotionCache}>
      <Head>
        <title>My page</title>
        <meta name="viewport" content="minimum-scale=1, initial-scale=1, width=device-width"/>
      </Head>
      <ThemeProvider theme={lightTheme}>
        <CssBaseline/>
          <SWRConfig value={{ fetcher: fetcher }}>
            <AuthContext.Provider value={{ user: user }}>
              <Component {...pageProps} />
            </AuthContext.Provider>
          </SWRConfig>
      </ThemeProvider>
    </CacheProvider>
  )
}
