import React, { Component } from "react";
import API from "../utils/API";
import { Col, Row, Container } from "../components/Grid";
import Hr from "../components/Hr";
import Button from "../components/Button";
import JSONPretty from "react-json-pretty";
import MainJumbotron from "../components/MainJumbotron";
import ImagesEndPoint from "../components/ImagesEndPoint";
// import { List, ListItem } from "../components/List";
// import { Link } from "react-router-dom";
// import DeleteBtn from "../components/DeleteBtn";
// import PostData from "../components/PostData";


import "./style.css";
class Landing extends Component {
    constructor(props) {
        super(props);
        this.state = {
            results: "",
            loading: false,
            postUrl: "https://icatcare.org/app/uploads/2018/07/Thinking-of-getting-a-cat.png",
        };
        this.myRef = React.createRef();
        this.visualTestsRef = React.createRef();
    }

    runTests = () => {
        this.setState({ loading: true });
        API.runTests()
            .then((res) => {
                console.log(this.visualTestsRef);
                this.setState({ results: res.data, loading: false });
                window.scrollTo({
                    top: this.visualTestsRef.current.parentNode.offsetTop,
                    behavior: "smooth",
                });
            })
            .catch((err) => console.log(err));
    };
   
    render() {
        return (
            <Container fluid>
                
                <MainJumbotron />
                
                <ImagesEndPoint myRef={this.myRef}/>
                
                <Hr />

                <Row></Row>
                <h4>Visual tests</h4>
                <p>Get insight on a batch of preselected images to see results</p>
                
                <Button onclick={this.runTests}>run visual tests</Button>
                {this.state.loading ? <i className="fa fa-circle-notch fa-spin spinner test-spinner"></i> : ""}
                
                <Row>
                    <Col size="md-6 ">
                        {this.state.results.length === 0 ? (
                            // "loading"
                            ""
                        ) : (
                            <div>
                                <h5 style={{ marginTop: 15 }}>Image mats</h5>

                                {this.state.results.map((image, i) => {
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
                        )}
                    </Col>
                    <Col size="md-6 ">
                        {this.state.results ? (
                            // <div className="details-container">
                            <div style={{ borderRadius: "5px" }} ref={this.visualTestsRef}>
                                {/* <kbd className="details "> */}
                                <h5 style={{ marginTop: 15 }}>Results sample</h5>
                                <div class="code">200</div>
                                <JSONPretty id="json-pretty" valueStyle="color:white" data={this.state.results}></JSONPretty>
                                {/* </kbd> */}
                            </div>
                        ) : (
                            <span></span>
                        )}
                    </Col>
                </Row>
            </Container>
        );
    }
}

export default Landing;
