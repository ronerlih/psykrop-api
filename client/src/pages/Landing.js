import React, { Component } from "react";
import DeleteBtn from "../components/DeleteBtn";
import Button from "../components/Button";
import API from "../utils/API";
import { Link } from "react-router-dom";
import { Col, Row, Container } from "../components/Grid";
import { List, ListItem } from "../components/List";
import { Input, TextArea, FormBtn } from "../components/Form";
import './style.css';
class Landing extends Component {
    state = {
      results: ""
    };

    componentDidMount() {}

    loadAPI = () => {};
    runTests = () => {
        API.runTests()
            .then(res => {
              this.setState({results: res})
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
                <Row>
                    <Col size="md-6">
                        <Button onclick={this.runTests}>run tests</Button>
                        {this.state.results.data ? (
                              <div className="details-container">
                                <kbd className="details ">
                                {this.state.results.data}
                              </kbd>
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
