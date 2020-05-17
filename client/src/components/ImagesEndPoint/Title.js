import React from "react";
import { Col, Row } from "../reusable/Grid";
export default function Title(){
    return (
        <div>
            <Row>
                <Col extraClass="endpoint-title">
                    <p></p>
                    <h4>/images endpoint</h4>
                    <p>A post request with an array of image urls will return an array of insights about each image.</p>
                </Col>
            </Row>
        </div>
    )
}