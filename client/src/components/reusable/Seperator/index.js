import React from "react";
import { Col, Row } from "../Grid";
import "./style.css";

function Seperator({title}) {
    return (
        <Row extraClass="jumbo seperator">
            <Col extraClass="" size="md-12">
                <div>
                    {title}
                </div>

            </Col>
        </Row>
    );
}

export default Seperator;
