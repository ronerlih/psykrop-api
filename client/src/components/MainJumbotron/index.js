import React from "react";
import { Col, Row } from "../reusable/Grid";
import BaseUrl from "./BaseUrl";

function MainJumbotron() {
   return (
      <Row extraClass="jumbo">
         <Col size="md-6" extraClass="order-last order-md-first ">
            <div>
               <span>
                  <strong className="page-headline">psyKrop API</strong> <br /> Is an non opinionated non biased A.I that returns insight about images, primarily an <em>Aesthetic Score</em>.
               </span>
               <BaseUrl />
               
            </div>
         </Col>
         <Col extraClass="text-right pt-1 " size="md-6">
            <div>
               {/* <span> </span> */}
               {/* <img src="https://img.shields.io/github/package-json/v/ronerlih/psykrop-api" alt="build badge - contiues deployment" /> */}

               <img src="https://api.travis-ci.com/ronerlih/psykrop-api.svg?branch=master" alt="build badge - contiues deployment" />
               <span> </span>
               <img src="https://img.shields.io/badge/release-alpha-cornflowerblue" alt="alpha badge" />
               <span> </span>
               {/* <a href='https://coveralls.io/github/ronerlih/psykrop-api?branch=master'> */}
               <img src="https://coveralls.io/repos/github/ronerlih/psykrop-api/badge.svg?branch=master" alt="Coverage Status" />
               {/* </a> */}
            </div>
            <div className="pt-1">
               <a href="https://www.psykrop.com/" target="_black">
                  About psyKrop
               </a>
            </div>
            <div>
               <a href="https://apps.apple.com/in/app/psykrop/id1398529702" target="_black">
                  iOS app
               </a>
            </div>
            <div>
               <a href="https://www.psykrop.com/try-on-web.html" target="_black">
                  Web widget
               </a>
            </div>
         </Col>
      </Row>
   );
}

export default MainJumbotron;
