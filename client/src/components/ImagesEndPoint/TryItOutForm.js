import React from "react";
import { Input } from "../Form";
import Button from "../Button";

export default function(props) {
    return (
        <div>
            <h5>Try it out:</h5>
            <span>Add comma seperated image urls (.JPG, JPEG, .PNG)</span>
            <Input value={props.postUrl} onChange={props.handleInputChange} name="postUrl" placeholder="image url" />
            <Button onclick={props.callPost} style={{ marginTop: 20 }}>
                call psyKrop api
            </Button>
            {props.postLoading ? <i className="fa fa-circle-notch fa-spin spinner "></i> : ""}
            <p></p>
        </div>
    );
}
