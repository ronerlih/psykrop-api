import React from "react";
import { Col, Row } from "../reusable/Grid";
import ResponseImages from "./ResponseImages";
import ResponseText from "./ResponseText";

export default function({ results, visualTestsRef }) {
    return (
        <>
        {results ? 
            <Row>
                <Col size="md-6" extraClass="order-last order-md-first">
                    {results
                        ? <ResponseImages results={results} />
                        : ""}
                </Col>
                <Col extraClass="bg-dark"  size="md-6 ">
                    {results
                        ? <ResponseText visualTestsRef={visualTestsRef} results={results}/>  
                        : "" }
                </Col>
            </Row>
            : <></> /* "loading"*/}
        </>
    );
}
