import React from "react";
import './style.css'

function Button(props) {
  return (
      <button ref={props.btnRef} type="button" style={props.style} className='btn btn-primary ' onClick={props.onclick} aria-label="test">
        {props.children}
      </button>
  );
}

export default Button;
