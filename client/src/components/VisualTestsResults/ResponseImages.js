import React from "react";
import { Col, Row, Container } from "../Grid";

export default function({ results }) {
    return (
        <div>
            <h5 style={{ marginTop: 15 }}>Image mats</h5>
            {results.map((image, i) => {
                return (
                    <Container key={i} style={{ border: "1px solid black" }} classes="images-container">
                        <Row>
                            <Col size="lg-4">
                                <img className="img-tests" src={"images/" + image.imageFeedback} alt="imageFeedback" />
                                <h6>image centers (insight)</h6>
                            </Col>
                            <Col size="lg-4">
                                <img className="img-tests" src={"images/" + image.edge} alt="edge" />
                                <h6>Edge mat</h6>
                            </Col>
                            <Col size="lg-4">
                                <img className="img-tests" src={"images/" + image.ratedPixels} alt="rated pixels" />
                                <h6>rated pixels mat</h6>
                            </Col>
                        </Row>
                        <Row>
                            <Col size="lg-4">
                                <img className="img-tests" src={"images/" + image.red_channel.url} alt="red channel" />
                                <h6>red mat</h6>
                            </Col>
                            <Col size="lg-4">
                                <img className="img-tests" src={"images/" + image.green_channel.url} alt="green channel" />
                                <h6>green mat</h6>
                            </Col>
                            <Col size="lg-4">
                                <img className="img-tests" src={"images/" + image.blue_channel.url} alt="blue channel" />
                                <h6>blue mat</h6>
                            </Col>
                        </Row>
                        <h6>imgID: {image.id}</h6>
                    </Container>
                );
            })}
        </div>
    );
}
