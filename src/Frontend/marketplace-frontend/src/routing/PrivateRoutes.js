import React from 'react';
import Box from "@mui/material/Box";
import CircularProgress from '@mui/material/CircularProgress';
import { Navigate } from 'react-router-dom';
import { useKeycloak } from '@react-keycloak/web';


export function PrivateRoute({ children }) {
    const {keycloak, initialized} = useKeycloak();

    return (
        initialized ? (
            keycloak.authenticated ? children : <Navigate to="/login"/>
        ) : (
            <Box sx={{textAlign: "center", margin: "15% 0 18px 0"}}>
                <CircularProgress color="basic" size={60}/>
            </Box>   
        )
    )
}