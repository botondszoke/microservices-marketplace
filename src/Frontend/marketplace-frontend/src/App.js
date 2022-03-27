import React from "react";
import {createTheme} from "@mui/material/styles";
import Controls from "./Controls.js";
import Home from "./Home.js";
import { Route, Routes } from "react-router-dom";
import Sales from "./Sales.js";
import { ThemeProvider } from "@emotion/react";

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
        }
      })
    }
  }

  render () {
    return (
      <ThemeProvider theme = {this.state.theme}>
        <Controls />
        <Routes>
          <Route exact path="/" element={<Home/>}/>
          <Route path="/sale/*" element={<Sales />}/>
        </Routes>
      </ThemeProvider>
    );
  }
}

export default App;
