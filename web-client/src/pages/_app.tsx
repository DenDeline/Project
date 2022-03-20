import {AppProps} from 'next/app'
import CssBaseline from '@material-ui/core/CssBaseline'
import Head from 'next/head'
import {ThemeProvider} from '@material-ui/core/styles'
import theme from '../theme'
import { useEffect } from 'react'


export default function MyApp(props: AppProps) {
  const {Component, pageProps} = props

  useEffect(() => {
    const jssStyles = document.querySelector('#jss-server-side')
    if (jssStyles?.parentElement) {
      jssStyles.parentElement.removeChild(jssStyles)
    }
  }, [])

  return (
    <>
      <Head>
        <title>My page</title>
        <meta name="viewport" content="minimum-scale=1, initial-scale=1, width=device-width"/>
      </Head>
      <ThemeProvider theme={theme}>
        <CssBaseline/>
        <Component {...pageProps} />
      </ThemeProvider>
    </>
  )
}
