import React from "react";
import JSONPretty from "react-json-pretty";

export default function() {
    return (
        <div>
            <h5>Request format</h5>
            <div className="code">POST /api/images?sort=[order]</div>
            BODY (in a json format - below, excepts also url encoded and text mime types)
            <JSONPretty id="json-pretty-body" valueStyle="color:white" data={{ images: ["url-to-img.file"] }}></JSONPretty>
        </div>
    );
}
