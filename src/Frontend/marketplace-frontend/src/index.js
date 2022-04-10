import React from 'react';
import ReactDOM from 'react-dom';
import {BrowserRouter} from 'react-router-dom';
import App from './App.js';
import reportWebVitals from './reportWebVitals';
import "./index.css";

ReactDOM.render(
  <meta name="viewport" content="initial-scale=1, width=device-width" />,
  document.head
)

ReactDOM.render(
  <title>BuYTE | Marketplace of the future</title>,
  document.head
)

ReactDOM.render(
  <React.StrictMode>
    <BrowserRouter>
      
      <App />
    </BrowserRouter>
  </React.StrictMode>,
  document.getElementById('root')
);


// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
