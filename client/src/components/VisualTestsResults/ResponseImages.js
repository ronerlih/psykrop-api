import React from "react";
import { Col, Row, Container } from "../reusable/Grid";

export default function({ results }) {
    return (
        <div>
            <h5 style={{ marginTop: 15 }}>Image mats</h5>
            {results.length ? results.map((image, i) => {
                return (
                    <Container key={i} style={{ border: "1px solid black" }} classes="images-container">
                        {image.red_channel
                            ? (<div><Row>
                                <Col size="lg-4">
                                    <img className="img-tests" src={"images/" + image.imageFeedback} alt="imageFeedback" />
                                    <span>image centers</span>
                                </Col>
                                <Col size="lg-4">
                                    <img className="img-tests" src={"images/" + image.edge} alt="edge" />
                                    <span>Edge mat</span>
                                </Col>
                                <Col size="lg-4">
                                    <img className="img-tests" src={"images/" + image.ratedPixels} alt="rated pixels" />
                                    <span>rated pixels mat</span>
                                </Col>
                            </Row>
                            <Row>
                                <Col size="lg-4">
                                    <img className="img-tests" src={"images/" + image.red_channel.url} alt="red channel" />
                                    <span>red mat</span>
                                </Col>
                                <Col size="lg-4">
                                    <img className="img-tests" src={"images/" + image.green_channel.url} alt="green channel" />
                                    <span>green mat</span>
                                </Col>
                                <Col size="lg-4">
                                    <img className="img-tests" src={"images/" + image.blue_channel.url} alt="blue channel" />
                                    <span>blue mat</span>
                                </Col>
                            </Row>
                            <h6 className="visual-images-container-id">Image id: {image.id}</h6>
                            </div>)
                        : ([<h6 className="visual-images-container-id">Image id: {image.id}</h6>, JSON.stringify(image)])
                            }
                    </Container>
                );
            }) 
        : <></>}
        </div>
    );
}
