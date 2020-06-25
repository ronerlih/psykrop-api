import React from "react";
import Button from "../reusable/Button";
import Title from "../reusable/Title";
import { Row, Col } from "../reusable/Grid";

export default function(props) {
  
const renderResponse = () => props.results ? (
   <><h5 style={{fontWeight:100, marginTop: 5, marginRight: 5, display: "inline-block" }} className="text-light">Request </h5>
      <pre>
         <code className="language-javascript">
{`fetch('https://psykrop-api.herokuapp.com/api/visualtests')
   .then( response => response.json())
   .then( data => console.log(data))`
}
         </code>
      </pre></>
   ) : "" ;

   return (
      <div>
               <Title info="endpoint" title="/api/visualtests" miniTitle="A GET request will return an array of insights from a predetermined set of images and will also provide visual output of the images in the set, the different channels rating, and the overall pixel ratings." />

         <Row style={{minHeight:150}}>
               <Col size="md-6" extraClass="mb-1" >
               {/* <h4>Visual tests</h4> */}
               <p>Get insights sample on a batch of preselected images, and see the image amalysis.</p>
               <Button onclick={props.runTests}>Run visual tests</Button>
               {props.loading ? <i className="fa fa-circle-notch fa-spin spinner test-spinner"></i> : ""}
            </Col>
            <Col size="md-6" extraClass="bg-dark_ only-on-large">
            {/* {renderResponse()} */}
            </Col>
         </Row>
      </div>
   );
}
