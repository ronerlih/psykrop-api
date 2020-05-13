// Load 'opencv.js' assigning the value to the global variable 'cv'
const cv = require("opencv.js");
const Jimp = require("jimp");
// console.log(cv.getBuildInformation())

// opencv methods
// console.log(Object.keys(cv.modules));
// opencv method search
// console.log(Object.keys(cv).filter(key => key.indexOf("INTER") >= 0));

module.exports = {
    getTestImages: function () {
        return [
            "https://2014.igem.org/wiki/images/a/a7/Sample.png",
            "https://images2.minutemediacdn.com/image/upload/c_crop,h_3236,w_5760,x_0,y_0/f_auto,q_auto,w_1100/v1554700227/shape/mentalfloss/istock-609802128.jpg",
            "https://i.ytimg.com/vi/MPV2METPeJU/maxresdefault.jpg",
            "https://qph.fs.quoracdn.net/main-qimg-7e3c3f89920a527c3becb8e312b0a465",
            "https://www.passmark.com/source/img_posts/montest_slide_2.png",
            "https://i.ytimg.com/vi/sr_vL2anfXA/maxresdefault.jpg",
            "https://upload.wikimedia.org/wikipedia/commons/1/16/HDRI_Sample_Scene_Balls_%28JPEG-HDR%29.jpg",
        ];
    },
    analyseImage: async function (image, id, saveImageLocaly) {
        return new Promise(async (resolve, reject) => {
            ///////-->
            // paramaters and mats

            const RED_COEFFICIENT = 1;
            const GREEN_COEFFICIENT = 0.8;
            const BLUE_COEFFICIENT = 1.2;
            //   const BLUR_SIZE = 3;
            const THRESHHOLD = 127.5;
            const CANNY_THRESHOLD = 100;
            const EDGE_WEIGHT = 0.719;
            const LOCATIONS_WEIGHT = 0.2;
            const EDGE_GAMMA = 0;
            const resultsOptions = {
                0: { channelName: "red_channel", color: [255, 0, 0] },
                1: { channelName: "green_channel", color: [0, 255, 0] },
                2: { channelName: "blue_channel", color: [0, 0, 255] },
                3: { channelName: "alpha_channel", color: [120, 120, 120] },
            };
            let resultObject = {};

            // prettier-ignore
            const locationMatrix = [
        0,0,0, 0,0,0, 0,0,0,
        0,0,0, 255,255,255, 0,0,0,
        0,0,0, 0,0,0, 0,0,0
      ];

            let COB;
            let arrayofMats = [];

            resultObject = {};
            let locationsMat = new cv.matFromArray(3, 3, cv.CV_8UC3, locationMatrix);
            let dst = new cv.Mat();
            let edgesMat = new cv.Mat();
            let channelMat = new cv.Mat();
            /////
            // read img

            // load local image file with jimp. It supports jpg, png, bmp, tiff and gif:
            try {
                var jimpSrc = await Jimp.read(image);
            } catch (e) {
                const error = new Error("broken url");
                resolve({
                    id: ("0" + id).slice(-2),
                    error: {
                        message: error.message,
                        stack: error.stack,
                        urlAttempted: image,
                    },
                });
            }
            // `jimpImage.bitmap` property has the decoded ImageData that we can use to create a cv:Mat
            let src = cv.matFromImageData(jimpSrc.bitmap);
            let zerosMat = new cv.Mat(src.rows, src.cols, cv.CV_8UC1, new cv.Scalar(0));
            let onesMat = new cv.Mat(src.rows, src.cols, cv.CV_8UC1, new cv.Scalar(255));
            let weightsMat = src.clone();

            // initaize point at img center
            COB = new cv.Point(parseInt(src.cols / 2), parseInt(src.rows / 2));

            //rgb -> gray
            cv.cvtColor(src, dst, cv.COLOR_RGBA2GRAY, 0);

            // inverse (initialize weightsMat)
            // cv.bitwise_not(src, dst);

            // flip rgb -> bgr
            // cv.cvtColor(dst, weightsMat, cv.COLOR_RGBA2BGR, 0);

            // blur
            // cv.blur(dst, dst, new cv.Size(BLUR_SIZE, BLUR_SIZE));

            ///////-->
            // rate edges
            // canny eadge detection
            cv.Canny(dst, edgesMat, CANNY_THRESHOLD, CANNY_THRESHOLD);

            // threshold
            cv.threshold(edgesMat, edgesMat, THRESHHOLD, 255, cv.THRESH_BINARY);

            // cnvrt back bfr save
            cv.cvtColor(edgesMat, edgesMat, cv.COLOR_GRAY2RGBA, 0);

            if (saveImageLocaly) {
                // saveImg(edgesMat, ("0" + id).slice(-2) + "-edge");
                resultObject.edge = ("0" + id).slice(-2) + "-edge.jpg";
            }

            // add to result
            resultObject.imgId = ("0" + id).slice(-2);

            await cv.addWeighted(weightsMat, 1 - EDGE_WEIGHT, edgesMat, EDGE_WEIGHT, EDGE_GAMMA, weightsMat, -1);

            ////rate locations
            // locations
            await cv.resize(locationsMat, locationsMat, weightsMat.size(), 0, 0, cv.INTER_LINEAR);
            cv.cvtColor(locationsMat, locationsMat, cv.COLOR_RGB2RGBA, 0);

            // saveImg(weightsMat, "weightsMatAfter");
            //  async function(_locationsMat){
            cv.addWeighted(weightsMat, 1 - LOCATIONS_WEIGHT, locationsMat, LOCATIONS_WEIGHT, EDGE_GAMMA, weightsMat, -1);

            if (saveImageLocaly) {
                // saveImg(weightsMat, ("0" + id).slice(-2) + "-rated-pixels");
                resultObject.ratedPixels = ("0" + id).slice(-2) + "-rated-pixels.jpg";
            }

            ////
            //split img
            arrayofMats = [];
            let channelsCenters = [];
            let vecOfMats = new cv.MatVector();
            cv.split(weightsMat, vecOfMats);
            for (let channelIndex = 0; channelIndex < 4; channelIndex++) {
                arrayofMats.push(vecOfMats.get(channelIndex));
            }
            arrayofMats.forEach((channel, channelIndex) => {
                // saveImg(img, "location2");
                let arr = new cv.moments(channel);
                let vecToMerge = new cv.MatVector();

                // COB
                COB = new cv.Point(arr.m10 / arr.m00, arr.m01 / arr.m00);

                //DRAW ON SRC
                if (channelIndex != 3) {
                    channelsCenters.push(COB);
                    if (saveImageLocaly) {
                        cv.circle(src, COB, 5, new cv.Scalar(0, 0, 0, 255), 5, cv.LINE_8, 0);
                        cv.circle(src, COB, 4, new cv.Scalar(resultsOptions[channelIndex].color[0], resultsOptions[channelIndex].color[1], resultsOptions[channelIndex].color[2], 255), 5, cv.LINE_8, 0);

                        COB.x = COB.x.toFixed(2);
                        COB.y = COB.y.toFixed(2);
                        // cv.putText(src,COB.x.toFixed(2) + ", " + COB.y.toFixed(2), new cv.Point(COB.x + 10, COB.y - 10),0,1, new cv.Scalar(255,0,0),cv.LINE_8,0, false);
                    }
                }

                // channel object
                resultObject[resultsOptions[channelIndex].channelName] = {};

                // write channel centers
                resultObject[resultsOptions[channelIndex].channelName].COB = COB;

                // save balance percent
                [resultObject[resultsOptions[channelIndex].channelName].distanceToCenter, resultObject[resultsOptions[channelIndex].channelName].balancePercent] = calcBalancePercentage(src, COB);

                // TO-DO:

                // distancesResultObject

                // optional: SSE support (Streaming SIMD Extensions)
                // optional: GPU support

                // get 8 distances =>

                // 2.Vertical: x=width/2
                // 3. Horizontal: y=height/2
                // 4. DIAG: y=-(height/width)*x+height
                // 5. ANTID: y=(height/width)*x
                // 6. RoT1: x=width/3
                // 7. RoT2: x=(2*width)/3
                // 8. RoT3: y=height/3
                // 9. RoT4: y=(2*height)/3

                // add to distancesResultObject

                //  distancesResultObject <= getMinimum
                //  distancesResultObject <= getAverage
                //  distancesResultObject <= getWeightedAverage

                // draw colored lines and distances (for visual testing)
                // draw colored distance lines and and text (distance value)

                // add to channel object

                // add moments to result
                resultObject[resultsOptions[channelIndex].channelName].imageMoments = {
                    m00: arr.m00,
                    m01: arr.m01,
                    m10: arr.m10,
                };
                // save channel
                if (saveImageLocaly) {
                    switch (resultsOptions[channelIndex].channelName) {
                        case "red_channel":
                            vecToMerge.push_back(channel);
                            vecToMerge.push_back(zerosMat);
                            vecToMerge.push_back(zerosMat);
                            vecToMerge.push_back(onesMat);
                            break;
                        case "green_channel":
                            vecToMerge.push_back(zerosMat);
                            vecToMerge.push_back(channel);
                            vecToMerge.push_back(zerosMat);
                            vecToMerge.push_back(onesMat);

                            break;
                        case "blue_channel":
                            vecToMerge.push_back(zerosMat);
                            vecToMerge.push_back(zerosMat);
                            vecToMerge.push_back(channel);
                            vecToMerge.push_back(onesMat);

                            break;
                        default:
                    }
                    if (channelIndex != 3) {
                        try {
                            cv.merge(vecToMerge, channelMat);
                        } catch (e) {
                            console.log(e);
                        }
                        if (saveImageLocaly) {
                            cv.cvtColor(channel, channel, cv.COLOR_GRAY2RGBA, 0);
                            // saveImg(channelMat, ("0" + id).slice(-2) + "-" + resultsOptions[i][0]);
                        }
                    }

                    resultObject[resultsOptions[channelIndex].channelName].url = ("0" + id).slice(-2) + "-" + resultsOptions[channelIndex].channelName + ".jpg";
                }
                channel.delete();
                vecToMerge.delete();

                arr = null;
            });
            if (saveImageLocaly) {
                // saveImg(src, ("0" + id).slice(-2) + "-image-feedback");
                resultObject.imageFeedback = ("0" + id).slice(-2) + "-image-feedback.jpg";
            }

            // average balance
            const aveCenter = weightedAverageThree(...channelsCenters);
            [resultObject.distanceToCenter, resultObject.balanceAllCoefficients] = calcBalancePercentage(src, aveCenter);

            //get avareg color
            resultObject.averageRGBColor = cv.mean(src).slice(0, 3);

            // delete mats
            src.delete();
            zerosMat.delete();
            onesMat.delete();
            dst.delete();
            weightsMat.delete();
            edgesMat.delete();
            channelMat.delete();
            locationsMat.delete();

            resolve({
                id: resultObject.imgId,
                balanceAllCoefficients: resultObject.balanceAllCoefficients,
                imageFeedback: resultObject.imageFeedback,
                distanceToCenter: resultObject.distanceToCenter,
                url: resultObject.url,
                edge: resultObject.edge,
                ratedPixels: resultObject.ratedPixels,
                averageRGBColor: resultObject.averageRGBColor,
                red_channel: resultObject.red_channel,
                green_channel: resultObject.green_channel,
                blue_channel: resultObject.blue_channel,
            });

            function weightedAverageThree(_redPoint, _greenPoint, _bluePoint) {
                return new cv.Point(
                    (_redPoint.x * RED_COEFFICIENT + _greenPoint.x * GREEN_COEFFICIENT + _bluePoint.x * BLUE_COEFFICIENT) / (RED_COEFFICIENT + GREEN_COEFFICIENT + BLUE_COEFFICIENT),
                    (_redPoint.y * RED_COEFFICIENT + _greenPoint.y * GREEN_COEFFICIENT + _bluePoint.y * BLUE_COEFFICIENT) / (RED_COEFFICIENT + GREEN_COEFFICIENT + BLUE_COEFFICIENT)
                );
            }
        });
        function calcBalancePercentage(mat, point) {
            let totalDistance = Math.sqrt((mat.cols / 2) * (mat.cols / 2) + (mat.rows / 2) * (mat.rows / 2));

            let diff = Math.sqrt((point.x - mat.cols / 2) * (point.x - mat.cols / 2) + (point.y - mat.rows / 2) * (point.y - mat.rows / 2));
            return [diff, 100 * (1 - diff / totalDistance)];
        }

        async function saveImg(mat, imgName) {
            return new Promise((resolve, reject) => {
                try {
                    new Jimp({
                        width: mat.cols,
                        height: mat.rows,
                        data: Buffer.from(mat.data),
                    }).write(`images/${imgName}.jpg`);
                    // mat.delete();
                    resolve(imgName);
                } catch (e) {
                    console.log("error caught: ");
                    console.log(e);
                    reject(e);
                }
            });
        }
    },
};
