// Load 'opencv.js' assigning the value to the global variable 'cv'
const cv = require("opencv.js");
const Jimp = require("jimp");
// console.log(cv.getBuildInformation())

// opencv methods
// console.log(Object.keys(cv.modules));

import Path from "path";

module.exports = {
    getTestImages: function() {
        return [
            "https://2014.igem.org/wiki/images/a/a7/Sample.png",
            "https://images2.minutemediacdn.com/image/upload/c_crop,h_3236,w_5760,x_0,y_0/f_auto,q_auto,w_1100/v1554700227/shape/mentalfloss/istock-609802128.jpg",
            "https://i.ytimg.com/vi/MPV2METPeJU/maxresdefault.jpg",
            "https://qph.fs.quoracdn.net/main-qimg-7e3c3f89920a527c3becb8e312b0a465",
            "https://www.passmark.com/source/img_posts/montest_slide_2.png",
            "https://i.ytimg.com/vi/sr_vL2anfXA/maxresdefault.jpg",
            "https://upload.wikimedia.org/wikipedia/commons/1/16/HDRI_Sample_Scene_Balls_%28JPEG-HDR%29.jpg"
        ];
    },
    analyseImage: async function(image, id, saveImageLocaly) {
        return new Promise(async (resolve, reject) => {
            setTimeout(async () => {
                ///////-->
                // paramaters and mats
                const RED_COEFFICIENT = 1;
                const GREEN_COEFFICIENT = 0.8;
                const BLUE_COEFFICIENT = 1.2;
                const BLUR_SIZE = 3;
                const THRESHHOLD = 127.5;
                const CANNY_THRESHOLD = 100;
                const EDGE_WEIGHT = 0.719;
                const LOCATIONS_WEIGHT = 0.2;
                const EDGE_GAMMA = 0;
                const resultsOptions = {
                    0: ["red_channel", [255, 0, 0]],
                    1: ["green_channel", [0, 255, 0]],
                    2: ["blue_channel", [0, 0, 255]],
                    3: ["alpha_channel", [120, 120, 120]]
                };
                let resultObject = {};
                // log cv object
                // console.log(Object.keys(cv).filter(key => key.indexOf("INTER") >= 0));

                /////
                // read img
                let path;
                saveImageLocaly ? (path = Path.resolve(__dirname, "../images/", image)) : (path = image);

                // load local image file with jimp. It supports jpg, png, bmp, tiff and gif:
                var jimpSrc = await Jimp.read(path);
                // `jimpImage.bitmap` property has the decoded ImageData that we can use to create a cv:Mat
                let src = cv.matFromImageData(jimpSrc.bitmap);
                let zerosMat = new cv.Mat(src.rows, src.cols, cv.CV_8UC1, new cv.Scalar(0));
                let onesMat = new cv.Mat(src.rows, src.cols, cv.CV_8UC1, new cv.Scalar(255));
                let dst = new cv.Mat();
                let weightsMat = src.clone();
                let edgesMat = new cv.Mat();
                let channelMat = new cv.Mat();
                // prettier-ignore
                const locationMatrix = [
                    0,0,0, 0,0,0, 0,0,0,
                    0,0,0, 255,255,255, 0,0,0,
                    0,0,0, 0,0,0, 0,0,0
                ];
                let locationsMat = new cv.matFromArray(3, 3, cv.CV_8UC3, locationMatrix);

                // initaize point at img center
                let centerPoint = new cv.Point(parseInt(src.cols / 2), parseInt(src.rows / 2));

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
                    saveImg(edgesMat, ("0" + id).slice(-2) + "-edge");
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
                    saveImg(weightsMat, ("0" + id).slice(-2) + "-rated-pixels");
                    resultObject.ratedPixels = ("0" + id).slice(-2) + "-rated-pixels.jpg";
                }

                ////
                //split img
                let arrayofMats = [];
                let channelsCenters = [];
                let vecOfMats = new cv.MatVector();
                cv.split(weightsMat, vecOfMats);
                for (let i = 0; i < 4; i++) {
                    arrayofMats.push(vecOfMats.get(i));
                }
                arrayofMats.forEach((channel, i) => {
                    // saveImg(img, "location2");
                    let arr = new cv.moments(channel);
                    let vecToMerge = new cv.MatVector();

                    centerPoint = new cv.Point(arr.m10 / arr.m00, arr.m01 / arr.m00);

                    //DRAW ON SRC
                    if (i != 3) {
                        if (saveImageLocaly) {
                            cv.circle(src, centerPoint, 5, new cv.Scalar(0, 0, 0, 255), 5, cv.LINE_8, 0);
                            cv.circle(src, centerPoint, 4, new cv.Scalar(resultsOptions[i][1][0], resultsOptions[i][1][1], resultsOptions[i][1][2], 255), 5, cv.LINE_8, 0);
                        } // cv.putText(src,centerPoint.x.toFixed(2) + ", " + centerPoint.y.toFixed(2), new cv.Point(centerPoint.x + 10, centerPoint.y - 10),0,1, new cv.Scalar(255,0,0),cv.LINE_8,0, false);
                        channelsCenters.push(centerPoint);
                    }
                    if (saveImageLocaly) {
                        centerPoint.x = centerPoint.x.toFixed(2);
                        centerPoint.y = centerPoint.y.toFixed(2);
                    }
                    // channel object
                    resultObject[resultsOptions[i][0]] = {};

                    // balance percent
                    let balancePercent = calcBalancePercentage(src, centerPoint);
                    resultObject[resultsOptions[i][0]].balancePercent = balancePercent;
                    
                    // save centers
                    resultObject[resultsOptions[i][0]].centerPoint = centerPoint;
                    centerPoint = new cv.Point(arr.m10 / arr.m00, arr.m01 / arr.m00);
                    
                    // add moments to result
                    resultObject[resultsOptions[i][0]].imageMoments = {
                        m00: arr.m00,
                        m01: arr.m01,
                        m10: arr.m10
                    }
                    // save channel
                    if (saveImageLocaly) {
                        switch (resultsOptions[i][0]) {
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
                        if (i != 3) {
                            try {
                                cv.merge(vecToMerge, channelMat);
                            } catch (e) {
                                console.log(e);
                            }
                            if (saveImageLocaly) {
                                cv.cvtColor(channel, channel, cv.COLOR_GRAY2RGBA, 0);
                                saveImg(channelMat, ("0" + id).slice(-2) + "-" + resultsOptions[i][0]);
                            }
                        }

                        resultObject[resultsOptions[i][0]].url = ("0" + id).slice(-2) + "-" + resultsOptions[i][0] + ".jpg";
                    }
                });
                if (saveImageLocaly) {
                    saveImg(src, ("0" + id).slice(-2) + "-image-feedback");
                    resultObject.imageFeedback = ("0" + id).slice(-2) + "-image-feedback.jpg";
                }
                

                // average balance
                const aveCenter = weightedAverageThree(...channelsCenters);
                resultObject.balanceAllCoefficients = calcBalancePercentage(src, aveCenter);

                //get avareg color
                resultObject.averageRGBColor = cv.mean(src).slice(0,3);
                
                // delete mats
                src.delete();
                zerosMat.delete();
                onesMat.delete();
                dst.delete();
                weightsMat.delete();
                edgesMat.delete();
                channelMat.delete();
                locationsMat.delete();

                const result = {
                    id: resultObject.imgId,
                    balanceAllCoefficients: resultObject.balanceAllCoefficients,
                    averageRGBColor: resultObject.averageRGBColor,
                    red_channel: resultObject.red_channel,
                    green_channel: resultObject.green_channel,
                    blue_channel: resultObject.blue_channel,
                }

                resolve(result);

                function weightedAverageThree(_redPoint, _greenPoint, _bluePoint) {
                    return new cv.Point(
                        (_redPoint.x * RED_COEFFICIENT + _greenPoint.x * GREEN_COEFFICIENT + _bluePoint.x * BLUE_COEFFICIENT) / (RED_COEFFICIENT + GREEN_COEFFICIENT + BLUE_COEFFICIENT),
                        (_redPoint.y * RED_COEFFICIENT + _greenPoint.y * GREEN_COEFFICIENT + _bluePoint.y * BLUE_COEFFICIENT) / (RED_COEFFICIENT + GREEN_COEFFICIENT + BLUE_COEFFICIENT)
                    );
                }
            }, 1000);
        });
        function calcBalancePercentage(mat, point) {
            let totalDistance = Math.sqrt((mat.cols / 2) * (mat.cols / 2) + (mat.rows / 2) * (mat.rows / 2));

            let diff = Math.sqrt((point.x - mat.cols / 2) * (point.x - mat.cols / 2) + (point.y - mat.rows / 2) * (point.y - mat.rows / 2));
            return 100 * (1 - diff / totalDistance);
        }

        async function saveImg(mat, imgName) {
            return new Promise((resolve, reject) => {
                try {
                    new Jimp({
                        width: mat.cols,
                        height: mat.rows,
                        data: Buffer.from(mat.data)
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
    }
};
