import Prism from "prismjs";
import React, { Component } from "react";
import API from "../../utils/API";
import { Col, Row } from "../reusable/Grid";
import Title from "../reusable/Title";
import ResponseInfo from "./ResponseInfo";
import RequestInfo from "./RequestInfo";
import TryItOutForm from "./TryItOutForm";
import PostResults from "./PostResults";
import "../../pages/prism.css";
import "./style.css";

class ImagesEndPoint extends Component {
	constructor(props) {
		super(props);
		this.state = {
			results: "",
			postLoading: false,
			postUrl: "https://icatcare.org/app/uploads/2018/07/Thinking-of-getting-a-cat.png",
			myRef: props.myRef,
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
		this.setState({ [name]: value });
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

	componentDidUpdate(){
		Prism.highlightAll();
	}
	renderResponse(){
		return this.state.postResults ? (
			<><h5 style={{fontWeight:100, marginTop: 5, marginRight: 5, display: "inline-block" }} className="text-light">Request </h5>
				<pre>
					<code className="language-javascript">
{`axios.post(
	'https://psykrop-api.herokuapp.com/api/images?sort=desc', 
	['https://icatcare.org/app/uploads/2018/07/Thinking-of-getting-a-cat.png']
	)
	.then( response => console.log(response))`}

					</code>
				</pre></>
			) : ""
	}
	render() {
		return (
			<div>
				<Title info="endpoint" title="/api/images" miniTitle="A POST request with an array of image urls will return an array of insights about each image." />
				<Row>
					<Col size="md-6">
						<h5 style={{ marginTop: 5, marginRight: 5, display: "inline-block" }}>Try it out </h5>
						<TryItOutForm postLoading={this.state.postLoading} callPost={this.callPost} postUrl={this.state.postUrl} handleInputChange={this.handleInputChange} />
					</Col>
					<Col size="md-6" extraClass="bg-dark">
					{this.renderResponse()}
					</Col>
				</Row>
				<Row>
					<Col size="md-6 ">
						<RequestInfo />
						<ResponseInfo />
					</Col>
					<Col size="md-6" extraClass="bg-dark results-to-scroll-to" name="results-to-scroll-to">
						{this.state.postResults ? (
							<PostResults myRef={this.state.myRef} data={this.state.postResults.data}>
								{this.state.postResults.status}
							</PostResults>
						) : (
							""
						)}
					</Col>
				</Row>
			</div>
		);
	}
}
export default ImagesEndPoint;
