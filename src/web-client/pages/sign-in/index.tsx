import React from "react";
import RandExp from "randexp";
import {GetServerSideProps} from "next";
import cookie from "cookie"
import { createHash } from 'crypto'

export const getServerSideProps: GetServerSideProps = async ({req, res}) => {
    const responseType = "code";
    const clientId =  process.env.CLIENT_ID ?? "";
    const redirectUri = process.env.REDIRECT_URI ?? "";
    
    
    const state = new RandExp("[A-Za-z0-9-._~]{32}").gen();
    const codeVerifier = new RandExp("[A-Za-z0-9-._~]{43,128}").gen();
    
    const hash = createHash("sha256");
    hash.update(codeVerifier, "ascii");
    const codeChallenge =  hash.digest().toString("base64");
    
    const codeChallengeMethod = "S256";
    
    res.setHeader(  
    "Set-Cookie", [
            cookie.serialize("state", state, {
                httpOnly: true,
                secure: process.env.NODE_ENV !== "development",
                maxAge: 3600,
                path: "/"
            }),
            
            cookie.serialize("code_verifier", codeVerifier, {
                httpOnly: true,
                secure: process.env.NODE_ENV !== "development",
                maxAge: 3600,
                path: "/"
            })
        ]
    );
    
    return {
        redirect: {
            statusCode: 302,
            destination: `https://localhost:44307/oauth2/authorize?response_type=${encodeURIComponent(responseType)}&client_id=${encodeURIComponent(clientId)}&redirect_uri=${encodeURIComponent(redirectUri)}&state=${encodeURIComponent(state)}&code_challenge=${encodeURIComponent(codeChallenge)}&code_challenge_method=${encodeURIComponent(codeChallengeMethod)}`,
        }
    }
}
    
const SignIn: React.FC = () => {
    return <></>;
}

export default SignIn;