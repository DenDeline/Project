import { StyledEngineProvider, Theme, ThemeProvider } from '@mui/material/styles'

import {AppProps} from 'next/app'
import CssBaseline from '@mui/material/CssBaseline'
import Head from 'next/head'
import theme from '../theme'
import { useEffect } from 'react'

declare module '@mui/styles/defaultTheme' {
  interface DefaultTheme extends Theme {}
}


export default function MyApp(props: AppProps) {
  const {Component, pageProps} = props

  useEffect(() => {
    const jssStyles = document.querySelector('#jss-server-side')
    if (jssStyles?.parentElement) {
      jssStyles.parentElement.removeChild(jssStyles)
    }
  }, [])

  return <>
    <Head>
      <title>My page</title>
      <meta name="viewport" content="minimum-scale=1, initial-scale=1, width=device-width"/>
    </Head>
    <StyledEngineProvider injectFirst>
      <ThemeProvider theme={theme}>
        <CssBaseline/>
        <Component {...pageProps} />
      </ThemeProvider>
    </StyledEngineProvider>
  </>
}
