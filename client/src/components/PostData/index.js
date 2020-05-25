import React from "react";
import "./style.css";
import JSONPretty from "react-json-pretty";

// This file exports both the List and ListItem components

export default class PostData extends React.Component {
   constructor(props) {
      super(props);
      this.myRef = React.createRef();
      this.props = props;
   }
   render() {
      return (
         <div>
            {this.props.state.postResults ? (
               <div ref={this.myRef} style={{ borderRadius: "5px" }}>
                  <h5 style={{ marginTop: 5, marginRight: 5, display: "inline-block" }}>Response </h5>
                  <div className="code">{this.state.postResults.status}</div>
                  <JSONPretty id="json-pretty" valueStyle="color:white" data={this.state.postResults.data}></JSONPretty>
               </div>
            ) : (
               <span></span>
            )}
         </div>
      );
   }
}
