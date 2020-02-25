import React, { Component } from "react";
import DeleteBtn from "../components/DeleteBtn";
import Button from "../components/Button";
import API from "../utils/API";
import { Link } from "react-router-dom";
import { Col, Row, Container } from "../components/Grid";
import { List, ListItem } from "../components/List";
import { Input, TextArea, FormBtn } from "../components/Form";
import JSONPretty from "react-json-pretty";

import "./style.css";
class Landing extends Component {
    state = {
        results: "",
        loading: false
    };

    componentDidMount() {}

    loadAPI = () => {};
    runTests = () => {
        this.setState({loading: true})
        API.runTests()
            .then(res => {
                this.setState({ results: res.data, loading: false });
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
                        <h4>Test Dashboard</h4>
                    </Col>
                </Row>
                <Button onclick={this.runTests}>run tests</Button>
                {this.state.loading 
                    ? <i className="fa fa-circle-notch fa-spin spinner"></i>
                    : ""}
                 <Row>

                    <Col size="md-6">
                        {this.state.results ? (
                            // <div className="details-container">
                            <div style={{marginTop:"10px", borderRadius:"5px"}}>
                                {/* <kbd className="details "> */}
                                <JSONPretty
                                    id="json-pretty"
                                    valueStyle="color:white"
                                    data={this.state.results}
                                ></JSONPretty>
                                {/* </kbd> */}
                            </div>
                        ) : (
                            <span></span>
                        )}
                    </Col>
                    <Col size="md-6">
                        {this.state.results.length === 0 ? (
                            // "loading"
                            ""
                        ) : (
                            <div>
                                {this.state.results.map(image => {
                                    return (<div>
                                        <h6>imgID: {image.imgId}</h6>
                                        <img className="img-tests" src={"images/" + image.imageFeedback} />
                                        <h6>image centers (insight)</h6>
                                        <img className="img-tests" src={"images/" + image.edge} />
                                        <h6>Edge mat</h6>
                                        <img className="img-tests" src={"images/" + image.ratedPixels} />
                                        <h6>rated pizels mat</h6>
                                        <img className="img-tests" src={"images/" + image.red_channel.url} />
                                        <h6>red mat</h6>
                                        <img className="img-tests" src={"images/" + image.green_channel.url} />
                                        <h6>green mat</h6>
                                        <img className="img-tests" src={"images/" + image.blue_channel.url} />
                                        <h6>blue mat</h6>
                                        <hr></hr>
                                    </div>)
                                })}
                            </div>
                        )
                        }
                    </Col>
                </Row>
            </Container>
        );
    }
}

export default Landing;
