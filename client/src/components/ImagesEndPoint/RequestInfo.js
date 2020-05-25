import React from "react";
import JSONPretty from "react-json-pretty";

export default function() {
    return (
        <div>
            <h5 style={{ marginTop: 5, marginRight: 5, display: "inline-block" }}>Request format </h5>
            <div className="code">POST /api/images?order=desc</div>
            Request BODY expample (application/json content type).
            <JSONPretty id="json-pretty-body" valueStyle="color:white" data={["url-to-img-01.jpg", "url-to-img-02.png"]}></JSONPretty>
            <span >
            Or use comma seperated for form/urlencoded content type.
            </span>
            <p></p>
        </div>
    );
}
