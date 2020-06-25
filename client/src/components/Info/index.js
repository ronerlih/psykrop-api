import React from "react";
import P from "../reusable/P";
import SectionTitle from "../reusable/Title/index.js";
import MainTitle from "../reusable/Title";
import { Row, Col } from "../reusable/Grid";

export default () => {
	return (
		<Row>
			<Col size="md-6">
				{/* <SectionTitle>Overview</SectionTitle> */}
                <MainTitle title="Overview"></MainTitle>
				<P>
					The psyKrop API returns an Aesthetic Score for an image based on its compositional strength as compared with established works of art and design (maximum score: 100). This Aesthetic Score can be used to rank large collections of images or combined with other aesthetic ranking
					algorithms for more comprehensive results. It is recommended that the Aesthetic Score is used to compare images with similar content when used to highlight best framing.
				</P>
				<P>
					The psyKrop API also reports the relative compositional strength (maximum score: 100) of the image as related to those structural lines (vertical/horizontal/diagonal) that are most dominant (two most dominant scores are reported). For image id [00] {`{`}the cat image{`}`} below, for
					example, the API shows image compositional dominance in the horizontal and diagonal direction.
				</P>
				<P>The API also reports the image strength (maximum score: 100) relative to the image center and provides the coordinates (pixels) for the compositional focal point or, COB (center of balance), of the image.</P>

				<SectionTitle>Pricing Plan</SectionTitle>
				<P>
					Free: API call/day=50 - Cost/month=$0
					<br />
					Basic: API call/day=500 - Cost/month=$50
					<br />
					Pro: API call/day=5k - Cost/month=$150
					<br />
					Ultimate: API call/day=50k - Cost/month=$500
				</P>
				<SectionTitle>Commercial Usage</SectionTitle>
				<P>
                All paid API plans are free of commercial use restrictions. Free API plans may be used in commercial services or products with attribution of "Powered by psyKrop™ " and must also link to this API website.
				</P>
                <SectionTitle>About us</SectionTitle>
				<P>
                Contact Aestatix LLC at <a href="https://aestatix.com">Aestatix.com</a>
                </P>
			</Col>
		</Row>
	);
};
