import React from "react";
import { Input } from "../reusable/Form";
import Button from "../reusable/Button";

export default function(props) {
    return (
        <div>
            <form onSubmit={props.callPost}>

            <span>Add comma seperated image urls (.JPG, .JPEG, .PNG)</span>
            <Input value={props.postUrl} onChange={props.handleInputChange} name="postUrl" placeholder="image url" />
            {/* <h5>Try it out:</h5> */}
            <Button style={{ display:"inline",marginTop: 5}} type="submit" >
                call psyKrop api
            </Button>
            {props.postLoading ? <i className="fa fa-circle-notch fa-spin spinner "></i> : ""}
            <p></p>

            </form>
        </div>
    );
}
