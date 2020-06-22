import React from "react";
import { Col, Row } from "./Grid";
export default function Title({ title, miniTitle, info }) {
	return (
		<Row>
			<Col size="md-6" style={{ position: "relative" }}>
				<div className="endpoint-title" style={{ position: "relative" }}>
					<h4 className="m-0" style={{ display: "inline-block" }}>
						{title}
					</h4>
					<span className="info-span"> {info}</span>
					<p>{miniTitle}</p>
				</div>
			</Col>

			<Col size="md-6" extraClass="bg-dark only-on-large"></Col>
		</Row>
	);
}
