import React from "react";
import RandExp from "randexp";
import {sha256} from "js-sha256";
import {GetServerSideProps} from "next";

export const getServerSideProps: GetServerSideProps = async (context) => {
    const responseType = "code";
    const clientId =  "project_next-js_8f62ee4312924427b386026f83028dff";
    const redirectUri = "http://localhost:3000/";
    const state = new RandExp("[A-Za-z0-9._~]{43,128}").gen();
    const codeVerifier = new RandExp("[A-Za-z0-9._~]{43,128}").gen();
    const codeChallenge = btoa(sha256(codeVerifier));
    const codeChallengeMethod = "S256";
    return {
        redirect: {
            statusCode: 302,
            destination: `https://localhost:44307/authorize?response_type=${encodeURIComponent(responseType)}&client_id=${encodeURIComponent(clientId)}&redirect_uri=${encodeURIComponent(redirectUri)}&state=${encodeURIComponent(state)}&code_challenge=${encodeURIComponent(codeChallenge)}&code_challenge_method=${encodeURIComponent(codeChallengeMethod)}`,
        }
    }
}
    
const SignIn: React.FC = () => {
    return <></>;
}

export default SignIn;