import React, { Component } from "react";
import API from "../../utils/API";
import { Col, Row } from "../reusable/Grid";
import Title from "./Title";
import ResponseInfo from "./ResponseInfo";
import RequestInfo from "./RequestInfo";
import TryItOutForm from "./TryItOutForm";
import PostResults from "./PostResults";
import "./style.css";

class ImagesEndPoint extends Component {
    constructor(props) {
        super(props);
        this.state = {
            results: "",
            postLoading: false,
            postUrl: "https://icatcare.org/app/uploads/2018/07/Thinking-of-getting-a-cat.png",
            myRef: props.myRef
        };
    }

    callPost = () => {
        this.setState({ postLoading: true });
        API.callPost(this.state.postUrl)
            .then((res) => {
                this.setState({ postResults: res, postLoading: false });
                window.scrollTo({
                    top: this.state.myRef.current.parentNode.offsetTop,
                    behavior: "smooth",
                });
            })
            .catch((err) => console.log(err));
    };

    handleInputChange = (event) => {
        const { name, value } = event.target;
        this.setState({[name]: value});
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
            <div>
                <Title />
                <Row>
                    <Col size="md-6 ">
                        <TryItOutForm 
                            postLoading={this.state.postLoading} 
                            callPost={this.callPost} 
                            postUrl={this.state.postUrl} 
                            handleInputChange={this.handleInputChange} />
                        <RequestInfo />
                        <ResponseInfo />
                    </Col>
                    <Col size="md-6" extraClass="results-to-scroll-to" name="results-to-scroll-to">
                    {this.state.postResults ? (
                        <PostResults 
                            myRef={this.state.myRef} 
                            data={this.state.postResults.data}>
                                {this.state.postResults.status}
                        </PostResults>) : ""}
                    </Col>
                </Row>
            </div>
        );
    }
}
export default ImagesEndPoint;
