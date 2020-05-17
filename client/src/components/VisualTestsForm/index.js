import React from "react";
import Button from "../reusable/Button";
import { Row } from "../reusable/Grid";

export default function(props) {
    return (
        <div>
            <Row></Row>
            <h4>Visual tests</h4>
            <p>Get insight on a batch of preselected images to see results</p>
            <Button onclick={props.runTests}>run visual tests</Button>
            {props.loading ? <i className="fa fa-circle-notch fa-spin spinner test-spinner"></i> : ""}
        </div>
    );
}
