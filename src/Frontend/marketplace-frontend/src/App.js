import React from "react";
import {createTheme} from "@mui/material/styles";
import Controls from "./Controls.js";
import Details from "./Details.js";
import Home from "./Home.js";
import Products from "./Products.js"
import { /*Navigate,*/ Route, Routes } from "react-router-dom";
import { PrivateRoute } from "./PrivateRoutes.js";
import Sales from "./Sales.js";
import { ThemeProvider } from "@emotion/react";

//import { useKeycloak } from "@react-keycloak/web";

function App() {

  const theme = createTheme({
    palette: {
      basic: {
        main: '#990222',
        contrastText: '#ededed'
      },
      white: {
        main: '#ededed'
      },
      odd: {
        main: '#c378f5'
      },
      tonalOffset: 0.2,
      contrastThreshold: 3,
    },
  });

  return (
    <ThemeProvider theme = {theme}>
      <Controls />
      <Routes>
        <Route exact path="/" element={/*<Navigate to="/sales"/>*/<Home />}/>
        <Route path="/myproducts/*" element={<PrivateRoute><Products /></PrivateRoute>}/>
        <Route path="/sales/*" element={<PrivateRoute><Sales /></PrivateRoute>}/>
        <Route path="/upload/*" element={<PrivateRoute><Details mode="upload" /></PrivateRoute>}/>
      </Routes>
    </ThemeProvider>
  );
}

export default App;
