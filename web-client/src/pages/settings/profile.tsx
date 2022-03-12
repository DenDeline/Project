import {AuthProps, withAuth} from "../../lib/auth"
import React from "react"
import Layout from "../../components/Layout"

export const getServerSideProps = withAuth(async () => {
  return {
    props: {}
  }
}, {withRedirect: true})

const ProfileSettings: React.FC<AuthProps> = ({data, error}) => {
  return (
    <Layout user={data?.user} title={'Profile config'}>
      <h2>Upload documents</h2>
      <form encType={'multipart/form-data'} method={'post'}>
        <dl>
          <dt>
            <label htmlFor={'profileImage'}>Image</label>
          </dt>
          <dd>
            <input id={'profileImage'} name={'profileImage'} type={'file'}/>
          </dd>
        </dl>
        <input type={'submit'} value={'Submit'}/>
      </form>
    </Layout>
  )
}

export default ProfileSettings
