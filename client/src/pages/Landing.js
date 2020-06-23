import React, { Component } from "react";
import API from "../utils/API";
import { Container } from "../components/reusable/Grid";
import MainJumbotron from "../components/MainJumbotron";
import Seperator from "../components/reusable/Seperator";
import ImagesEndPoint from "../components/ImagesEndPoint";
import VisualTestsForm from "../components/VisualTestsForm";
import VisualTestsResults from "../components/VisualTestsResults";
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
                {/* <Seperator title="Images Endpoint"/> */}
                <ImagesEndPoint myRef={this.myRef} />
                {/* <Seperator title="Visual Tests Endpoint"/> */}
                <VisualTestsForm runTests={this.runTests} loading={this.state.loading} />
                <VisualTestsResults visualTestsRef={this.visualTestsRef} results={this.state.results} />
            </Container>
        );
    }
}
export default Landing;
