import {GetServerSidePropsContext, GetServerSidePropsResult} from "next";
import {ApplicationUser} from "../models/user";
import axios from "axios";

export interface AuthConfig {
    withRedirect: boolean,
    roles: string[]
}

export interface AuthProps<P = {}> {
    data?: { user: ApplicationUser } & P
    error?: {
        status: number,
        message: string
    }
}

export type GetServerSidePropsContextWithUser = GetServerSidePropsContext & { req: { user: ApplicationUser }}
export type Callback<P> = (context: GetServerSidePropsContextWithUser) => Promise<GetServerSidePropsResult<P>>

export const withAuth =<P>(callback: Callback<P>, config?: Partial<AuthConfig>) => {
    return async (context: GetServerSidePropsContext): Promise<GetServerSidePropsResult<AuthProps<P>>> => {

        try {
            const { req } = context;

            if (!req.cookies.access_token) {
                //TODO: Implement refresh token
                if (config?.withRedirect){
                    return {
                        redirect: {
                            destination: '/sign-in',
                            statusCode: 302
                        }
                    }
                }
                else {
                    return {
                        props: {
                            error: {
                                message: 'Forbidden',
                                status: 302
                            }
                        }
                    }
                }

            }

            let { data: user } = await axios.get<ApplicationUser>('https://localhost:5001/api/user', {
                headers: {
                    Authorization: `Bearer ${req.cookies.access_token}`
                }
            })

            console.log(user)

            context.req = Object.defineProperty(context.req, 'user', { value: user })

            const result = await callback(context as GetServerSidePropsContextWithUser);

            if ('props' in result){
                return {
                    props: {
                        data: {
                            ...result.props,
                            user
                        }
                    }
                }
            }
            return result;
        }
        catch ({response: {status, data}}) {
            return {
                props: {
                    error: {
                        status,
                        message: data
                    }
                }
            }
        }

    };
}