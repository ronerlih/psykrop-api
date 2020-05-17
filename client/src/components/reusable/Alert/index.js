import React from "react";
import './style.css'

function Alert(props) {
  console.log(props.cpu)
  return (
    <div className={props.cpu ? "alert alert-info alert-dismissible" : "alert alert-info alert-dismissible fadeOut"} role="alert">
      Running on core {props.cpu}
      <button type="button" className="close" onClick={props.onclick} aria-label="Close">
        <span aria-hidden="true">&times;</span>
      </button>
    </div>
  );
}

export default Alert;
