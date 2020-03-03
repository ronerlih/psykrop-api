import React from "react";
import ReactDOM from "react-dom";
import App from "./App";

// check if dev mode
process
    ? ReactDOM.render(<App />, document.getElementById("root"))
    : ReactDOM.hydrate(<App />, document.getElementById("root"))