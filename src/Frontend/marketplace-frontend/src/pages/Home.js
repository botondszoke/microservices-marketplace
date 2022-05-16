import React from "react";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import Container from "@mui/material/Container";
import Divider from "@mui/material/Divider";
import { Navigate } from "react-router-dom";
import Typography from "@mui/material/Typography";
import { useKeycloak } from '@react-keycloak/web';
import { CircularProgress } from "@mui/material";


export default function Home() {
    const {keycloak, initialized} = useKeycloak();
    console.log(document.cookie);
    if (initialized) {
        return(
            keycloak.authenticated ? <Navigate to="/sales" /> :
            <Container>
                <Box sx={{textAlign: "center", margin: "30% 0 18px 0"}}>
                    <Typography variant="h3"><b>BuYTE</b></Typography>
                    <Typography variant="h6" sx={{margin: "12px 0"}}>To check our latest products, please log in or sign up!</Typography>
                </Box>
                <Divider />
                <Box sx={{textAlign: "center", margin: "18px 0"}}>
                    <Button color="basic" variant="contained" size="large" onClick={() => keycloak.login()}>
                        Proceed to login
                    </Button>
                </Box>
            </Container>
        );
    }
    return (
        <Box sx={{textAlign: "center", margin: "15% 0 18px 0"}}>
            <CircularProgress color="basic" size={60}/>
        </Box>
    )
}
