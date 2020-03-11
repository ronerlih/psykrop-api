import React, { Component } from "react";
// import DeleteBtn from "../components/DeleteBtn";
import Button from "../components/Button";
import API from "../utils/API";
// import { Link } from "react-router-dom";
import { Col, Row, Container } from "../components/Grid";
import Hr from "../components/Hr";
// import { List, ListItem } from "../components/List";
import { Input, TextArea, FormBtn } from "../components/Form";
import JSONPretty from "react-json-pretty";

import "./style.css";
class Landing extends Component {
    state = {
        results: "",
        loading: false,
        postLoading: false,
        postUrl: "https://via.placeholder.com/400"
    };

    componentDidMount() {}

    loadAPI = () => {};
    runTests = () => {
        this.setState({ loading: true });
        API.runTests()
            .then(res => {
                this.setState({ results: res.data, loading: false });
            })
            .catch(err => console.log(err));
    };
    callPost = () => {
        this.setState({ postLoading: true });
        API.callPost(this.state.postUrl)
            .then(res => {
                this.setState({ postResults: res, postLoading: false });
            })
            .catch(err => console.log(err));
    };
    deleteDoc = id => {
        API.deleteDoc(id)
            .then(res => this.loadBooks())
            .catch(err => console.log(err));
    };

    handleInputChange = event => {
        const { name, value } = event.target;
        this.setState({
            [name]: value
        });
    };

    handleFormSubmit = event => {
        event.preventDefault();
        if (this.state.title && this.state.author) {
            API.saveDOC({
                title: this.state.title,
                author: this.state.author,
                synopsis: this.state.synopsis
            })
                .then(res => this.loadBooks())
                .catch(err => console.log(err));
        }
    };

    render() {
        return (
            <Container fluid>
                <Row>
                    <Col size="md-6">
                        <h4>Base URL</h4>
                        <p>Make all API calls to </p>
                        <url>https://psykrop-api.herokuapp.com/</url>
                    </Col>
                </Row>
                <Hr/>

                <Row>
                    <Col size="md-6">
                        <p></p>
                        <h4>/images endpoint</h4>
                        <p>Post request with an array of image resouces - urls, will return an array of insight objects about each image.</p>
                    </Col>
                </Row>
                <Row>
                    <Col size="md-6 ">
                        <h5>request endpoint:</h5>
                        <code>POST /api/images?sorted=[*order]</code>
                        <p>options</p>
                        <h5>try in sandbox:</h5>
                        <span>add comma seperated image resouces urls (.JPG, JPEG, .PNG)</span>
                        <Input value={this.state.postUrl} onChange={this.handleInputChange} name="postUrl" placeholder="image url" />
                        <Button onclick={this.callPost} style={{ marginTop: 20 }}>
                            call psyKrop api
                        </Button>

                        {this.state.postLoading ? <i className="fa fa-circle-notch fa-spin spinner"></i> : ""}
                        
                        <p></p>
                        <h5>response endpoints:</h5>
                        <ul>
                            <li>
                                <strong>balanceAllCoefficients</strong>: image balance percent (0-100)
                            </li>
                            <li>
                                <strong>distanceToCenter</strong>: mean balance point distance to image center (in pixels)
                            </li>
                            <li>
                                <strong>averageRGBColor</strong>: average color (r,g,b) value
                            </li>
                            <li>
                                <strong>red_channel</strong>:
                                <ul>
                                    <li>balancePercent: channel balance percent (0-100)</li>
                                    <li>distanceToCenter: weighted mean balance point distance to image center (in pixels)</li>
                                    <li>centerPoint: weighted mean center Point</li>
                                    <li>imageMoments: image moments point [Array]</li>
                                </ul>
                            </li>
                            <li>
                                <strong>green_channel</strong>: same data points for the green channel
                            </li>
                            <li>
                                <strong>blue_channel</strong>: same data points for the blue channel
                            </li>
                        </ul>

                        <p></p>
                    </Col>
                    <Col size="md-6">
                {this.state.postResults ? (
                            // <div className="details-container">
                            <div style={{ borderRadius: "5px" }}>
                                {/* <kbd className="details "> */}
                                <h5 style={{ marginTop:5, marginRight: 5, display:"inline-block"}}>Response </h5>
                                <code >
                                    {this.state.postResults.status}
                                </code>

                                <JSONPretty id="json-pretty" valueStyle="color:white" data={this.state.postResults.data}></JSONPretty>
                                {/* </kbd> */}
                            </div>
                        ) : (
                            <span></span>
                        )}
                    </Col>
                </Row>
                <Hr/>

                <Row>
                
                </Row>
                <h4>Visual tests</h4>
                <Button onclick={this.runTests} style={{ marginTop: 20 }}>
                    run visual tests
                </Button>
                {this.state.loading ? <i className="fa fa-circle-notch fa-spin spinner"></i> : ""}
                <Row>
                    <Col size="md-6 ">
                        {this.state.results ? (
                            // <div className="details-container">
                            <div style={{ borderRadius: "5px" }}>
                                {/* <kbd className="details "> */}
                                <h6 style={{ marginTop: 5 }}>Results sample</h6>

                                <JSONPretty id="json-pretty" valueStyle="color:white" data={this.state.results}></JSONPretty>
                                {/* </kbd> */}
                            </div>
                        ) : (
                            <span></span>
                        )}
                    </Col>
                    <Col size="md-6 ">
                        {this.state.results.length === 0 ? (
                            // "loading"
                            ""
                        ) : (
                            <div>
                                <h6 style={{ marginTop: 5 }}>Image mats</h6>

                                {this.state.results.map(image => {
                                    return (
                                        <Container style={{ border: "1px solid black" }} classes="images-container">
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
                </Row>
            </Container>
        );
    }
}

export default Landing;
