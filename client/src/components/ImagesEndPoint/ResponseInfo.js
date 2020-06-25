import React from "react";
export default function() {

    const OptionSpan = ({opt}) => 
        <>
            <span style={{ color: "#09b107" }}>/api/images?order={opt}</span> 
            <span style={{ display:"block",fontSize:13}}>(Array of image urls in the request body)</span>
        </> 

    return (
        <div style={{marginBottom:150}}>
            {/* options info */}
            <h5>Options</h5>
            <ul>
                <li>
                    Optionally order results acording to thier <em>Aesthetic score</em>.
                    <br />
                    <ul>
                        <li>
                            ASC order: <br/> 
                            <OptionSpan opt="asc"></OptionSpan> 
                        </li>
                        <li>
                            DESC order: <br/>
                            <OptionSpan opt="desc"></OptionSpan> 
                        </li>
                    </ul>
                </li>
            </ul>
            {/* datapoints info  */}
            <h5>Response data points: (TBD)</h5>
            {/* <ul style={{ color: "#ddd" }}> */}
            <ul >
                <li>
                    <strong>aesthetic_score</strong>: Number (0-100) : based on its compositional strength as compared with established works of art and design, read more below.
                </li>
                <li>
                    <strong>horizontal_strength</strong>: Number (0-100) :  relative compositional strength compared to the horizontal line, read more below.
                </li>
                <li>
                    <strong>diagonal_strength_01</strong>: Number (0-100) :  relative compositional strength compared to the diagonal line, read more below.
                </li>
                <li>
                    <strong>center_strength</strong>: Number (0-100) :  relative compositional strength compared to the image center, read more below.
                </li>
                <li>
                    <strong>center_of_balance</strong>: Point(x, y) : Mean balance point of the rated pixel after analysis transformations.</li>
                </ul>
                
                {/* <li>
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
             */}
            {/* error info */}
            <h5>Error messages</h5>
            broken or unprocessed link will return status 200 (ok) in the response, the image response data will display the eror message.
        
        </div>
    );
}
