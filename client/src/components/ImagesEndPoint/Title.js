import React from "react";
import { Col, Row } from "../reusable/Grid";
export default function Title(){
    return (
        <div>
            <Row>
                <Col extraClass="endpoint-title">
                    <h4 className="m-0" style={{display:"inline-block"}}>/api/images</h4>
                    <span className="info-span"> endpoint</span>
                    <p>A post request with an array of image urls will return an array of insights about each image.</p>
                </Col>
            </Row>
        </div>
    )
}