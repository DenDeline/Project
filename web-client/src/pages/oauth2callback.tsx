import {GetServerSideProps} from "next"
import React from "react"
import axios from "axios"
import querystring from "querystring"
import cookie from "cookie"

interface AccessTokenResponse {
  access_token: string,
  token_type: string,
  expires_in: number
}


export const getServerSideProps: GetServerSideProps = async ({req, res, query}) => {

  const stateFromCookies = req.cookies.state
  const codeVerifierFromCookies = req.cookies.code_verifier

  const codeToken = query.code as string
  const stateFromAuthServer = query.state

  if (stateFromAuthServer !== stateFromCookies) {
    // TODO: Create endpoint for error messages

    res.setHeader(
      "Set-Cookie",
      [
        cookie.serialize("state", "", {maxAge: 0}),
        cookie.serialize("code_verifier", "", {maxAge: 0})
      ]
    )

    return {
      redirect: {
        statusCode: 302,
        destination: "http://localhost:3000"
      }
    }
  }

  const postData = {
    grant_type: "authorization_code",
    code: codeToken,
    redirect_uri: process.env.REDIRECT_URI ?? "",
    client_id: process.env.CLIENT_ID,
    code_verifier: codeVerifierFromCookies
  }

  const response = await axios.post<AccessTokenResponse>("https://localhost:7045/oauth2/token", querystring.stringify(postData))

  res.setHeader(
    "Set-Cookie",
    [
      cookie.serialize("access_token", response.data.access_token, {
        httpOnly: true,
        secure: process.env.NODE_ENV !== "development",
        maxAge: response.data.expires_in,
        path: "/"
      }),
      cookie.serialize("state", "", {maxAge: 0}),
      cookie.serialize("code_verifier", "", {maxAge: 0})
    ]
  )

  return {
    redirect: {
      statusCode: 302,
      destination: "http://localhost:3000"
    }
  }
}

const Oauth2callback: React.FC = () => {
  return <></>
}

export default Oauth2callback

