import React from 'react';
import CircularProgress from '@mui/material/CircularProgress';
import { Navigate } from 'react-router-dom';
import { useKeycloak } from '@react-keycloak/web';


export function PrivateRoute({ children }) {
    const {keycloak, initialized} = useKeycloak();

    //const isLoggedIn = keycloak.authenticated;

    return (
        initialized ? (
            keycloak.authenticated ? children : <Navigate to="/"/>
        ) : (
            <CircularProgress />   
        )
    )
}