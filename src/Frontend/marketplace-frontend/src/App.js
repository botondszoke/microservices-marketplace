import React from "react";
import {createTheme} from "@mui/material/styles";
import Controls from "./Controls.js";
import Home from "./Home.js";
import MyProducts from "./MyProducts.js"
import { Navigate, Route, Routes } from "react-router-dom";
import Sales from "./Sales.js";
import { ThemeProvider } from "@emotion/react";
import Upload from "./Upload.js";

class App extends React.Component {

  constructor(props) {
    super(props);
    this.state = {
      theme: createTheme({
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
        
      })
    }
  }

  render () {
    return (
      <ThemeProvider theme = {this.state.theme}>
        <Controls />
        <Routes>
          <Route exact path="/" element={<Navigate to="/sales"/>}/>
          <Route path="/myproducts/*" element={<MyProducts />}/>
          <Route path="/sales/*" element={<Sales />}/>
          <Route path="/upload/*" element={<Upload />}/>
        </Routes>
      </ThemeProvider>
    );
  }
}

export default App;
