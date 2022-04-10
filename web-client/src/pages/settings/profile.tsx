import {withAuth} from '@sentaku/lib'

import { Layout } from '@sentaku/components'
import { NextPage } from 'next'

export const getServerSideProps = withAuth(async () => {
  return {
    props: {}
  }
}, {withRedirect: true})

const ProfileSettings: NextPage = () => {
  return (
    <Layout title={'Profile config'}>
      <h2>Upload documents</h2>
      <form encType={'multipart/form-data'} method={'post'} action={'/api/user/profileImage'} >
        <dl>
          <dt>
            <label htmlFor={'profileImage'}>Image</label>
          </dt>
          <dd>
            <input id={'profileImage'} name={'FormFile'} type={'file'}/>
          </dd>
        </dl>
        <input type={'submit'} value={'Submit'}/>
      </form>
    </Layout>
  )
}

export default ProfileSettings
