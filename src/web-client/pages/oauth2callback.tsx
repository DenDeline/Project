import {GetServerSideProps} from "next";




export const getServerSideProps: GetServerSideProps = async (context) => {
    return {
        redirect: {
            statusCode: 302,
            destination: "http://localhost:3000"
        }
    }
}

export default function Oauth2callback(){
    return <></>;
}

