import {GetServerSideProps} from "next";
import axios from "axios";
import React from "react";

interface SecretProps {
    payload: string
}

export const getServerSideProps: GetServerSideProps<SecretProps> = async (context) => {
    const config = {
        headers: {
            Authorization: `Bearer ${context.req.cookies.access_token}`
        }
    }
    
    const result = await axios.get("https://localhost:44307/api/auth/secret", config);
    
    return {
        props: {
            payload: result.data.payload
        }
    }
}

const Secret: React.FC<SecretProps> = (props) => {
    return (
        <>
            <div>
                <h1>Secret data</h1>
                <hr/>
                <div>
                    { props.payload }
                </div>
                
            </div>
        </>
    );
}

export default Secret;