import { GetServerSideProps, NextPage } from 'next'

import cookie from 'cookie'

export const getServerSideProps: GetServerSideProps = async ({req, res}) => {

  res.setHeader(
    'Set-Cookie',
    [
      cookie.serialize('access_token', '', {maxAge: 0, path: '/'})
    ]
  )

  return {
    redirect: {
      destination: '/',
      statusCode: 302
    }
  }
}

const Logout: NextPage = () => {
  return <></>
}

export default Logout
