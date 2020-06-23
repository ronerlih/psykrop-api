import React from "react";

export default function(props) {
	return (
		<div className="text-light json-results" ref={props.myRef} style={{ borderRadius: "5px" }}>
			<h5 style={{ fontWeight: 100, marginTop: 5, marginRight: 5, display: "inline-block" }}>Response </h5>
			<div className="code-dark" >{props.children}</div>
			<pre style={{minHeight: "calc(100vh - 110px )"}}>
				<code className="language-javascript">{JSON.stringify(props.data, null, 2)}</code>
			</pre>
		</div>
	);
}
