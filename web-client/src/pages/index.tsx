import Head from 'next/head'
import React from "react"
import Layout from "../components/Layout"
import {AuthProps, withAuth} from "../lib/auth"
import Navbar from "../components/Navbar"
import {Container} from "@material-ui/core"

export const getServerSideProps = withAuth(async () => {
  return {
    props: {}
  }
})

const Home: React.FC<AuthProps> = (props) => {

  return (
    <Layout title={'Home'} user={props.data?.user}>
      <Head>
        <title>Home</title>
        <link rel="icon" href="/favicon.ico"/>
        <meta name="description" content="Generated by create next app"/>
      </Head>
      <Navbar/>

    </Layout>
  )
}

export default Home
