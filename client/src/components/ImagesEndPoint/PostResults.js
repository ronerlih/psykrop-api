
import React from "react";
import JSONPretty from "react-json-pretty";

export default function(props) {
    return(
    <div className="text-light json-results" ref={props.myRef} style={{ borderRadius: "5px" }}>
        <h5 style={{fontWeight:100, marginTop: 5, marginRight: 5, display: "inline-block" }}>Response </h5>
        <div className="code">{props.children}</div>

        <JSONPretty id="json-pretty" valueStyle="color:white" data={props.data}></JSONPretty>
    </div>
    )
}
