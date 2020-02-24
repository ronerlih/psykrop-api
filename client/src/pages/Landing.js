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
        results: ""
    };

    componentDidMount() {}

    loadAPI = () => {};
    runTests = () => {
        API.runTests()
            .then(res => {
                this.setState({ results: res.data });
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
                                <img className="img-tests" src={"images/" + this.state.results.imageFeedback} />
                                <h6>image centers (insight)</h6>
                                {console.log(this.state.results.edge)}
                                <img className="img-tests" src={"images/" + this.state.results.edge} />
                                <h6>Edge mat</h6>
                                <img className="img-tests" src={"images/" + this.state.results.ratedPixels} />
                                <h6>rated pizels mat</h6>
                                <img className="img-tests" src={"images/" + this.state.results.red_channel.url} />
                                <h6>red mat</h6>
                                <img className="img-tests" src={"images/" + this.state.results.green_channel.url} />
                                <h6>green mat</h6>
                                <img className="img-tests" src={"images/" + this.state.results.blue_channel.url} />
                                <h6>blue mat</h6>

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
