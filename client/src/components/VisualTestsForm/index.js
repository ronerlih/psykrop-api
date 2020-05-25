import React from "react";
import Button from "../reusable/Button";
import Title from "../reusable/Title";
import { Row, Col } from "../reusable/Grid";

export default function(props) {
   return (
      <div>
         <Row>
            <Col size="md-6" extraClass="mb-1">
               <Title info="endpoint" title="/api/visualtests" miniTitle="A GET request will return an array of insights from a predetermined set of images and will also provide visual output of the images in the set, the different channels rating, and the overall pixel ratings." />
               {/* <h4>Visual tests</h4> */}
               <p>Get insight on a batch of preselected images to see results</p>
               <Button onclick={props.runTests}>Run visual tests</Button>
               {props.loading ? <i className="fa fa-circle-notch fa-spin spinner test-spinner"></i> : ""}
            </Col>
         </Row>
      </div>
   );
}
