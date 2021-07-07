import React from "react";
import Link from "next/link";

const Layout: React.FC = ({children}) => {
    return(
        <>
            <div>
                <ul>
                    <li>
                        <Link href={'/'}>Home</Link>
                    </li>
                    <li>
                        <Link href={'/sign-in'}>Sign-in</Link>
                    </li>
                    <li>
                        <Link href={'/secret'}>Secret</Link>
                    </li>
                </ul>
            </div>
            
            <div>
                {
                    children
                }
            </div>
            
        </>
    );
}

export default Layout;