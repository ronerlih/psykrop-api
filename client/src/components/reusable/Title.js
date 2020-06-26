import React from "react";
import { Col, Row } from "./Grid";
import corner from "./corner.svg";

const RENDER_CORNER = true;
const Corner = () => RENDER_CORNER ?
	<>
		<img src={corner} style={{zIndex:2,right:0,bottom:-21,position:"absolute", height:20}}/>
		<div style={{zIndex:2,right:0,bottom:-21,position:"absolute", height:20, width:"100%"}}/>
	</> : <></>
	
export default function Title({ title, miniTitle, info }) {
	return (
		<Row style={{position:"relative"}}>

			<Col size="md-6" style={{ position: "relative" }}>
				<div className="endpoint-title" style={{ position: "relative" }}>
					<h4 className="m-0" style={{ display: "inline-block" }}>
						{title}
					</h4>
					<Corner />
					<span className="info-span"> {info}</span>
					<p>{miniTitle}</p>
				</div>
				
				{RENDER_CORNER ? <div style={{zIndex: 2, right: -16, bottom: -21,position: "relative", width:"100%", height: 20,top: -10,borderRight: "1px solid black", background: "white"}}></div> : <></>}
			</Col>

			<Col size="md-6" extraClass="bg-dark_ only-on-large"></Col>
		</Row>
	);
}
