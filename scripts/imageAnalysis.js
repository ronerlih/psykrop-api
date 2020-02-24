// const cv = require("opencv4nodejs");

// Load 'opencv.js' assigning the value to the global variable 'cv'
const cv = require("opencv.js");
const Jimp = require("jimp");
// console.log(cv.getBuildInformation())

// console.log(Object.keys(cv.modules));
import Path from "path";

module.exports = {
    getTestImages: function() {
        return ["https://qph.fs.quoracdn.net/main-qimg-7e3c3f89920a527c3becb8e312b0a465"]
        return [
            "https://www.nasa.gov/sites/default/files/styles/full_width_feature/public/thumbnails/image/pia23533.jpg"
        ];
        return ["https://i.ytimg.com/vi/MPV2METPeJU/maxresdefault.jpg"];
    },
    analyseImage: async function(image) {
        return new Promise(async (resolve, reject) => {
            setTimeout(async () => {
                ///////-->
                // paramaters and mats
                const BLUR_SIZE = 3;
                const THRESHHOLD = 127.5;
                const CANNY_THRESHOLD = 100;
                const EDGE_WEIGHT = 0.719;
                const LOCATIONS_WEIGHT = 0.2;

                const EDGE_GAMMA = 0;

                const resultsOptions = {
                    0: ["red_channel", [255,0,0]],
                    1: ["green_channel", [0,255,0]],
                    2: ["blue_channel", [0,0,255]],
                    3: ["alpha_channel", [120,120,120]]
                }
                const moments = [];
                const WeightedCentroid = [];

                const path = Path.resolve(__dirname, "../images/", image);
                // load local image file with jimp. It supports jpg, png, bmp, tiff and gif:
                var jimpSrc = await Jimp.read(path);
                // `jimpImage.bitmap` property has the decoded ImageData that we can use to create a cv:Mat
                let src = cv.matFromImageData(jimpSrc.bitmap);
                let dst = new cv.Mat();
                // console.log(Object.keys(cv).filter(key => key.indexOf("INTER") >= 0));
                let mask = new cv.Mat.ones(dst.rows, dst.cols, cv.CV_8U)
                let weightsMat = src.clone();
                let edgesMat = new cv.Mat();
                // prettier-ignore
                const locationMatrix = [
                    0,0,0, 0,0,0, 0,0,0,
                    0,0,0, 255,255,255, 0,0,0,
                    0,0,0, 0,0,0, 0,0,0
                ];
                let locationsMat = new cv.matFromArray(3, 3, cv.CV_8UC3, locationMatrix);
                saveImg(locationsMat, "locations");

                // TO-DO: initaize with center of img point
                let centerPoint = new cv.Point(0, 0);
                
                //rgb -> gray
                cv.cvtColor(src, dst, cv.COLOR_RGBA2GRAY, 0);
                              
                // inverse (initialize weightsMat)
                // cv.bitwise_not(src, dst);
                
                // flip rgb -> bgr
                // cv.cvtColor(dst, weightsMat, cv.COLOR_RGBA2BGR, 0);
                                
                // mat to gray, TO-DO: extract 3 channels
                // cv.cvtColor(weightsMat, dst, cv.COLOR_BGR2GRAY, 0);


                // cv.cvtColor(weightsMat, weightsMat, cv.COLOR_BGR2RBG, 0);
                // saveImg(weightsMat, "weights")

                // blur
                // cv.blur(dst, dst, new cv.Size(BLUR_SIZE, BLUR_SIZE));

                ///////-->
                // rate edges
                // canny eadge detection
                cv.Canny(dst, edgesMat, CANNY_THRESHOLD, CANNY_THRESHOLD);

                // threshold
                cv.threshold(edgesMat, edgesMat, THRESHHOLD,255, cv.THRESH_BINARY);

                // cnvrt back bfr save
                cv.cvtColor(edgesMat, edgesMat, cv.COLOR_GRAY2RGBA, 0);
                saveImg(edgesMat, "edge");
                // saveImg(weightsMat, "weightsMat");

                await cv.addWeighted(weightsMat,1 - EDGE_WEIGHT, edgesMat,EDGE_WEIGHT,EDGE_GAMMA,weightsMat,-1);
                // saveImg(weightsMat, "weightsMatAfter");

                ////rate locations
                // locations
                await cv.resize(locationsMat, locationsMat, weightsMat.size(),0,0,cv.INTER_LINEAR);
                cv.cvtColor(locationsMat, locationsMat, cv.COLOR_RGB2RGBA, 0);

                // saveImg(weightsMat, "weightsMatAfter");
                //  async function(_locationsMat){
                cv.addWeighted(weightsMat,1 - LOCATIONS_WEIGHT, locationsMat,LOCATIONS_WEIGHT,EDGE_GAMMA,weightsMat,-1);
                
                saveImg(weightsMat, "loc")
            // });

                ////
                //split img
                let arrayofMats = [];
                let results = {};
                let vecOfMats = new cv.MatVector()
                cv.split(weightsMat, vecOfMats);
                for(let i = 0; i<4; i++){
                    arrayofMats.push(vecOfMats.get(i))
                }
                arrayofMats.forEach((channel, i) => {
                    // saveImg(img, "location2");
                    let arr = new cv.moments(channel);

                    centerPoint = new cv.Point(
                        arr.m10 / arr.m00,
                        arr.m01 / arr.m00
                    );
                    if(i != 3)
                    {cv.circle(src,centerPoint, 5, new cv.Scalar(0,0,0,255),5,cv.LINE_8,0)
                    cv.circle(src,centerPoint, 4, new cv.Scalar(resultsOptions[i][1][0],resultsOptions[i][1][1],resultsOptions[i][1][2],255),5,cv.LINE_8,0)
                    }
                    centerPoint.x = centerPoint.x.toFixed(2)
                    centerPoint.y = centerPoint.y.toFixed(2)
                    results[resultsOptions[i][0]] = centerPoint;
                    cv.cvtColor(channel, channel, cv.COLOR_GRAY2RGBA, 0);
                    saveImg(channel, resultsOptions[i][0]);

                    //DRAW ON SRC

                })
                saveImg(src,"src")
                // // retur to rgba for display
                // cv.cvtColor(dst, dst, cv.COLOR_GRAY2RGBA, 0);

                resolve("Center point: x: " + JSON.stringify(results, null, 2));
            }, 1000);
            // resolve("Center point: " + centerPoint.x);
        });

        async function saveImg(mat, imgName) {
            return new Promise((resolve, reject) => {
                try {
                    new Jimp({
                        width: mat.cols,
                        height: mat.rows,
                        data: Buffer.from(mat.data)
                    }).write(`${imgName}.jpg`);
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
