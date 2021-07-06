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