// const cv = require("opencv4nodejs");

// Load 'opencv.js' assigning the value to the global variable 'cv'
const cv = require("opencv.js");
const Jimp = require("jimp");
// console.log(cv.getBuildInformation())

// console.log(Object.keys(cv.modules));
import Path from "path";

module.exports = {
    getTestImages: function() {
        return [
            "https://www.nasa.gov/sites/default/files/styles/full_width_feature/public/thumbnails/image/pia23533.jpg"
        ];
        return ["https://i.ytimg.com/vi/MPV2METPeJU/maxresdefault.jpg"];
    },
    analyseImage: async function(image) {
        return new Promise(async (resolve, reject) => {
            setTimeout(async () => {
                const BLUR_SIZE = 3;
                const THRESHHOLD = 127.5;
                const CANNY_THRESHOLD = 100;
                const moments = [];
                const WeightedCentroid = [];

                const path = Path.resolve(__dirname, "../images/", image);
                // load local image file with jimp. It supports jpg, png, bmp, tiff and gif:
                var jimpSrc = await Jimp.read(path);
                // `jimpImage.bitmap` property has the decoded ImageData that we can use to create a cv:Mat
                let src = cv.matFromImageData(jimpSrc.bitmap);
                let dst = new cv.Mat();
                let edgesMat = new cv.Mat();
                let locationsMat = new cv.Mat();
                // TO-DO: initaize with center of img point
                let centerPoint = new cv.Point(0, 0);

                // inverse
                cv.bitwise_not(src, dst);

                // mat to gray, TO-DO: extract 3 channels
                cv.cvtColor(dst, dst, cv.COLOR_RGBA2GRAY, 0);

                // blur
                // cv.blur(dst, dst, new cv.Size(BLUR_SIZE, BLUR_SIZE));

                // rate edges
                // canny eadge detection
                cv.Canny(dst, edgesMat, CANNY_THRESHOLD, CANNY_THRESHOLD);

                // threshold
                cv.threshold(edgesMat, edgesMat, THRESHHOLD, 255, cv.THRESH_BINARY);
                
                // cnvrt back bfr save
                cv.cvtColor(edgesMat, edgesMat, cv.COLOR_GRAY2RGBA, 0);
                saveImg(edgesMat, "edge");
                //TO-DO: calc wieghted ave of mat and edges

                // cv.imgproc.moments(dst,arr, false);

                let arr = new cv.moments(dst);

                centerPoint = new cv.Point(
                    arr.m10 / arr.m00,
                    arr.m01 / arr.m00
                );
                console.log(centerPoint);
                // retur to rgba for display
                cv.cvtColor(dst, dst, cv.COLOR_GRAY2RGBA, 0);

                // // following lines is copy&paste of opencv.js dilate tutorial:
                // let M = cv.Mat.ones(5, 5, cv.CV_8U);
                // let anchor = new cv.Point(-1, -1);
                // cv.dilate(
                //     src,
                //     dst,
                //     M,
                //     anchor,
                //     10,
                //     cv.BORDER_CONSTANT,
                //     cv.morphologyDefaultBorderValue()
                // );
                // Now that we are finish, we want to write `dst` to file `output.png`. For this we create a `Jimp`
                // image which accepts the image data as a [`Buffer`](https://nodejs.org/docs/latest-v10.x/api/buffer.html).
                // `write('output.png')` will write it to disk and Jimp infers the output format from given file name:


                //     cv.imreadAsync("./images/img.jpg", (err, mat) => {
                //         const BLUR_SIZE = 3;
                //         const THRESHHOLD = 127.5;
                //         const CANNY_THRESHOLD = 100;

                //         const moments = [];
                //         const WeightedCentroid = [];
                //         let matCopy = mat;

                //         // To-DO: devide ot channels
                //         //mats
                //         mat = mat.bgrToGray();

                //         // inverse
                //         mat = mat.bitwiseNot();

                //         //blur
                //         mat = mat.blur(new cv.Size(BLUR_SIZE, BLUR_SIZE));

                //         // canny eadge detection
                //         // mat = mat.canny(CANNY_THRESHOLD, CANNY_THRESHOLD);

                //         //// mat to buffer
                //         let buffer = [];
                //         // for(let y = 0; y < mat.rows; y++){
                //         //     for(let x = 0; x < mat.cols; x++){
                //         //     // buffer.push(mat.at(y,x));
                //         //     if(mat.at(y,x) === 0){
                //         //         mat.set(y,x,1)
                //         //     }
                //         //     }
                //         // }
                //         // console.log(buffer);

                //         // mat = mat.mul(0.00392156862)
                //         //thereshold
                //         // mat = mat.threshold(THRESHHOLD, 255, cv.THRESH_BINARY);

                //         // //moments async
                //         // mat.momentsAsync(false)
                //         //     .then(res => {
                //         //         console.log(res);
                //         //         cv.imshow("img", mat);
                //         //         cv.waitKey();
                //         //         resolve("100%");
                //         //     })
                //         //     .catch(e => console.log(e));
                //         // moments.push(new cv.Moments(mat, false));
                //         // console.log(moments);
                //         cv.imshow("img", mat);
                //         cv.waitKey();
                //         resolve("100%");
                //         // moments.push();
                //         // console.log(moments);
                //         // WeightedCentroid.push(new cv.Point( + Math.floor(moments[0].m10 / moments[0].m00), + Math.floor(moments[0].m01 / moments[0].m00)));
                //         // console.log("center: " + WeightedCentroid[0].x +", " + WeightedCentroid[0].y);
                //     });
                // });
                resolve("Center point: " + centerPoint.x);
            }, 1000);
            // resolve("Center point: " + centerPoint.x);
        });

        async function saveImg(mat, imgName){
            return new Promise((resolve, reject) => {
                try {
                    new Jimp({
                        width: mat.cols,
                        height: mat.rows,
                        data: Buffer.from(mat.data)
                    }).write(`${imgName}.jpg`);
                    mat.delete();
                    resolve(imgName);
                } catch (e) {
                    console.log("error caught: ");
                    console.log(e);
                    reject(e);
                }
            })
        }
    }
};
