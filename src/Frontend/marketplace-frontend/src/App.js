import React from "react";
import {createTheme} from "@mui/material/styles";
import Controls from "./Controls.js";
import Home from "./Home.js";
import MyProducts from "./MyProducts.js"
import { /*Navigate,*/ Route, Routes } from "react-router-dom";
import { PrivateRoute } from "./PrivateRoutes.js";
import Sales from "./Sales.js";
import { ThemeProvider } from "@emotion/react";
import Upload from "./Upload.js";
//import { useKeycloak } from "@react-keycloak/web";

function App() {

  const theme = createTheme({
    palette: {
      basic: {
        main: '#990222',
        contrastText: '#ededed'
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
        <Route path="/myproducts/*" element={<PrivateRoute><MyProducts /></PrivateRoute>}/>
        <Route path="/sales/*" element={<Sales />}/>
        <Route path="/upload/*" element={<PrivateRoute><Upload /></PrivateRoute>}/>
      </Routes>
    </ThemeProvider>
  );
}

export default App;
