import React from "react";
export default function({ visualTestsRef, results }) {
	return (
		<div style={{ borderRadius: "5px", marginBottom: 15}} ref={visualTestsRef}>
			<h5 className="text-light" style={{ marginTop: 15 }}>Results sample</h5>
			<div className="code-dark long">200</div>
			<pre>
				<code className="language-javascript">
                    {JSON.stringify(results, null ,2)}
                </code>
			</pre>
		</div>
	);
}
