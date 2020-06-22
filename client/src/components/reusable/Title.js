import React from "react";
import { Col, Row } from "./Grid";
export default function Title({ title, miniTitle, info }) {
   return (
         <div className="endpoint-title">
            <h4 className="m-0" style={{ display: "inline-block" }}>
               {title}
            </h4>
            <span className="info-span"> {info}</span>
            <p>{miniTitle}</p>
         </div>
   );
}
