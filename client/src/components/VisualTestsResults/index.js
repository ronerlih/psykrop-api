import React from "react";
import { Col, Row } from "../Grid";
import ResponseImages from "./ResponseImages";
import ResponseText from "./ResponseText";

export default function({ results, visualTestsRef }) {
    return (
        <Row>
            <Col size="md-6 ">
                {console.log(results)}
                {results
                    ? <ResponseImages results={results} />
                    : /* "loading"*/ ""}
            </Col>
            <Col size="md-6 ">
                {results 
                    ? <ResponseText visualTestsRef={visualTestsRef} results={results}/>  
                    : "" }
            </Col>
        </Row>
    );
}
