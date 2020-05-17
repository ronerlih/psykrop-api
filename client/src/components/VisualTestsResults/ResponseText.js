import React from "react";
import JSONPretty from "react-json-pretty";
export default function({ visualTestsRef, results }) {
    return (
        <div style={{ borderRadius: "5px" }} ref={visualTestsRef}>
            <h5 style={{ marginTop: 15 }}>Results sample</h5>
            <div className="code">200</div>
            <JSONPretty id="json-pretty" valueStyle="color:white" data={results}></JSONPretty>
        </div>
    );
}
