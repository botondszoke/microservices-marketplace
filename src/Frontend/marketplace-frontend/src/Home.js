import React from "react";
import Box from "@mui/material/Box";
import { useKeycloak } from '@react-keycloak/web';

export default function Home() {
    const {keycloak, initialized} = useKeycloak();
    return(
        <Box className="asd" id="asd">
            Welcome at the homepage of BuYTE!
            {initialized ?
                keycloak.authenticated && <pre >{JSON.stringify(keycloak, undefined, 2)}</pre>
                : <h2>keycloak initializing ....!!!!</h2>
            }
        </Box>

    );
}
