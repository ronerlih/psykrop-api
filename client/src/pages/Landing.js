import React, { Component } from "react";
import API from "../utils/API";
import { Container } from "../components/Grid";
import Hr from "../components/Hr";
import MainJumbotron from "../components/MainJumbotron";
import ImagesEndPoint from "../components/ImagesEndPoint";
import VisualTestsForm from "../components/VisualTestsForm";
import VisualTestsResults from "../components/VisualTestsResults";
import "./style.css";
// import { List, ListItem } from "../components/List";
// import { Link } from "react-router-dom";
// import DeleteBtn from "../components/DeleteBtn";
// import PostData from "../components/PostData";

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
                <ImagesEndPoint myRef={this.myRef} />
                <Hr />
                <VisualTestsForm runTests={this.runTests} loading={this.state.loading} />
                <VisualTestsResults visualTestsRef={this.visualTestsRef} results={this.state.results} />
            </Container>
        );
    }
}
export default Landing;
