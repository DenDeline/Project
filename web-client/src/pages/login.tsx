import React from "react";
import Layout from "../components/Layout";
import {GetServerSideProps} from "next";

export const getServerSideProps: GetServerSideProps = async ({req} ) => {
    if (req.cookies.access_token){
        return {
            redirect: {
                statusCode: 302,
                destination: '/'
            }
        }
    }

    return {
        props: { }
    }
}



const Login: React.FC = () => {
    return (
        <Layout title={'Login'}>

        </Layout>
    )
}

export default Login;