import React, { Component } from "react";
import { Link, animateScroll as scroll } from "react-scroll";
// import DeleteBtn from "../components/DeleteBtn";
import Button from "../components/Button";
import API from "../utils/API";
// import { Link } from "react-router-dom";
import { Col, Row, Container } from "../components/Grid";
import Hr from "../components/Hr";
// import { List, ListItem } from "../components/List";
import { Input, TextArea, FormBtn } from "../components/Form";
import JSONPretty from "react-json-pretty";
import PostData from "../components/PostData";

import "./style.css";
class Landing extends Component {
    constructor(props) {
        super(props);
        this.state = {
            results: "",
            loading: false,
            postLoading: false,
            postUrl: "https://icatcare.org/app/uploads/2018/07/Thinking-of-getting-a-cat.png",
        };
        this.myRef = React.createRef();
        this.visualTestsRef = React.createRef();
    }

    componentDidMount() {}

    loadAPI = () => {};
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
    callPost = () => {
        this.setState({ postLoading: true });
        API.callPost(this.state.postUrl)
            .then((res) => {
                this.setState({ postResults: res, postLoading: false });
                // if(this.myRef.current)
                window.scrollTo({
                    top: this.myRef.current.parentNode.offsetTop,
                    behavior: "smooth",
                });
            })
            .catch((err) => console.log(err));
    };
    deleteDoc = (id) => {
        API.deleteDoc(id)
            .then((res) => this.loadBooks())
            .catch((err) => console.log(err));
    };

    handleInputChange = (event) => {
        const { name, value } = event.target;
        this.setState({
            [name]: value,
        });
    };

    handleFormSubmit = (event) => {
        event.preventDefault();
        if (this.state.title && this.state.author) {
            API.saveDOC({
                title: this.state.title,
                author: this.state.author,
                synopsis: this.state.synopsis,
            })
                .then((res) => this.loadBooks())
                .catch((err) => console.log(err));
        }
    };

    render() {
        return (
            <Container fluid>
                <Row extraClass="jumbo">
                    <Col size="md-6" extraClass="order-last order-md-first">
                        <div>
                            <span>
                                <strong class="page-headline">psyKrop API</strong> <br /> Is a non opinionated non bias A.I that returns insight about images, primarily an <em>Aesthetic balance score</em>.
                            </span>
                            <p></p>
                        </div>
                        <p></p>
                        <h4>Base URL</h4>
                        <p>Make all API calls to </p>
                        <div className="url">https://psykrop-api.herokuapp.com/</div>
                    </Col>
                    <Col extraClass="text-right pt-1 " size="md-6">
                    <div>
                            {/* <span> </span> */}
                            {/* <img src="https://img.shields.io/github/package-json/v/ronerlih/psykrop-api" alt="build badge - contiues deployment" /> */}
                            <img src="https://api.travis-ci.com/ronerlih/psykrop-api.svg?branch=master" alt="build badge - contiues deployment" />
                            <span> </span>
                            <img src="https://img.shields.io/badge/release-alpha-cornflowerblue" alt="alpha badge" />
                            {/* <span style={{ backgroundColor: "gray", padding: 3, fontSize: 8, color: "white" }}>[alpha]</span> */}
                        </div>
                        <div className="pt-1">
                            <a href="https://www.psykrop.com/" target="_black">
                                About psyKrop
                            </a>
                        </div>
                        <div>
                            <a href="https://apps.apple.com/in/app/psykrop/id1398529702" target="_black">
                                iOS app
                            </a>
                        </div>
                        <div>
                            <a href="https://www.psykrop.com/try-on-web.html" target="_black">
                                Web widget
                            </a>
                        </div>
                    </Col>
                </Row>

                <Row>
                    <Col extraClass=" endpoint-title">
                        <p></p>
                        <h4>/images endpoint</h4>
                        <p>Post request with an array of image resouces - urls, will return an array of insight objects about each image.</p>
                    </Col>
                </Row>
                <Row>
                    <Col size="md-6 ">
                        <h5>Try it out:</h5>
                        <span>Add comma seperated image urls (.JPG, JPEG, .PNG)</span>
                        <Input value={this.state.postUrl} onChange={this.handleInputChange} name="postUrl" placeholder="image url" />
                        <Button onclick={this.callPost} style={{ marginTop: 20 }}>
                            call psyKrop api
                        </Button>
                        {this.state.postLoading ? <i className="fa fa-circle-notch fa-spin spinner "></i> : ""}
                        <p></p>
                        <h5>Request format</h5>
                        <div className="code">POST /api/images?sort=[order]</div>
                        BODY (in json format below, excepts also url encoded, form, and text mime types)
                        <JSONPretty id="json-pretty-body" valueStyle="color:white" data={{ images: ["url-to-img.file"] }}></JSONPretty>
                        <h5>Options</h5>
                        <ul>
                            <li>
                                sort=[<span style={{ color: "#09b107" }}>order</span>] <span style={{ fontSize: 10, verticalAlign: 5 }}>[optional]</span>
                                <br />
                                order results acording to balance-harmony percentage.
                                <br />
                                <span style={{ color: "#09b107" }}>order</span> :
                                <ul>
                                    <li>descending</li>
                                    <li>ascending</li>
                                </ul>
                            </li>
                        </ul>
                        <h5>Response data points:</h5>
                        <ul>
                            <li>
                                <strong>balanceAllCoefficients</strong>: image balance-harmony percent (0-100)
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
                        <h5>Error messages</h5>
                        broken or unprocessed link will return status 200 (ok) in the response, the image response data will display the eror message.
                    </Col>
                    <Col size="md-6" extraClass="results-to-scroll-to" name="results-to-scroll-to" name="results-to-scroll-to">
                        {this.state.postResults ? (
                            // <div className="details-container">
                            <div ref={this.myRef} style={{ borderRadius: "5px" }}>
                                {/* <kbd className="details "> */}
                                <h5 style={{ marginTop: 5, marginRight: 5, display: "inline-block" }}>Response </h5>
                                <div className="code">{this.state.postResults.status}</div>

                                <JSONPretty id="json-pretty" valueStyle="color:white" data={this.state.postResults.data}></JSONPretty>
                                {/* </kbd> */}
                            </div>
                        ) : (
                            <span></span>
                        )}
                    </Col>
                </Row>
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
