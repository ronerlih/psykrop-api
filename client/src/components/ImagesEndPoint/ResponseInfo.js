import React from "react";
export default function() {
    return (
        <div>
            {/* options info */}
            <h5>Options</h5>
            <ul>
                <li>
                    Optionally you can order results acording to thier Aesthetic score by adding a sort parameter to the call.
                    <br />
                    <ul>
                        <li>
                            Ascending order: <span style={{ color: "#09b107" }}>https://psykrop-api.herokuapp.com/?order=ascending</span> 
                            <br/><em>(with an array of image urls in the request body)</em>
                        </li>
                        <li>
                            Descending order: <span style={{ color: "#09b107" }}>https://psykrop-api.herokuapp.com/?order=descending</span> 
                            <br/><em>(with an array of image urls in the request body)</em>
                        </li>
                    </ul>
                </li>
            </ul>
            {/* datapoints info  */}
            <h5>Response data points: (TBD)</h5>
            <ul style={{ color: "#ddd" }}>
                <li>
                    <strong>balanceAllCoefficients</strong>: image balance-harmony percent (0-100)
                </li>
                <li>
                    <strong>distanceToCenter</strong>: mean balance point distance to image center (in pixels)
                </li>
                <li>
                    <strong>averageRGBColor</strong>: average color (r,g,b) value
                </li>
                <li>
                    <strong>red_channel</strong>:
                    <ul>
                        <li>balancePercent: channel balance percent (0-100)</li>
                        <li>distanceToCenter: weighted mean balance point distance to image center (in pixels)</li>
                        <li>centerPoint: weighted mean center Point</li>
                        <li>imageMoments: image moments point [Array]</li>
                    </ul>
                </li>
                <li>
                    <strong>green_channel</strong>: same data points for the green channel
                </li>
                <li>
                    <strong>blue_channel</strong>: same data points for the blue channel
                </li>
            </ul>
            
            {/* error info */}
            <h5>Error messages</h5>
            broken or unprocessed link will return status 200 (ok) in the response, the image response data will display the eror message.
        </div>
    );
}
