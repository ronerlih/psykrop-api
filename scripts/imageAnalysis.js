// const cv = require("opencv4nodejs");

  // Load 'opencv.js' assigning the value to the global variable 'cv'
const cv = require('opencv.js');
const Jimp = require('jimp');
// console.log(cv.getBuildInformation())

// console.log(Object.keys(cv.modules));
import Path from "path";

module.exports = {
    getTestImages: function() {
        return["https://www.nasa.gov/sites/default/files/styles/full_width_feature/public/thumbnails/image/pia23533.jpg"];
        return ["https://i.ytimg.com/vi/MPV2METPeJU/maxresdefault.jpg"];
    },
    analyseImage: async function(image) {
            const path = Path.resolve(__dirname, "../images/", image);
              // load local image file with jimp. It supports jpg, png, bmp, tiff and gif:
            var jimpSrc = await Jimp.read('./images/img.jpg');
            // `jimpImage.bitmap` property has the decoded ImageData that we can use to create a cv:Mat
    var src = cv.matFromImageData(jimpSrc.bitmap);
    // following lines is copy&paste of opencv.js dilate tutorial:
  let dst = new cv.Mat();
  let M = cv.Mat.ones(5, 5, cv.CV_8U);
  let anchor = new cv.Point(-1, -1);
  cv.dilate(src, dst, M, anchor, 10, cv.BORDER_CONSTANT, cv.morphologyDefaultBorderValue());
    // Now that we are finish, we want to write `dst` to file `output.png`. For this we create a `Jimp`
  // image which accepts the image data as a [`Buffer`](https://nodejs.org/docs/latest-v10.x/api/buffer.html).
  // `write('output.png')` will write it to disk and Jimp infers the output format from given file name:
  new Jimp({
    width: dst.cols,
    height: dst.rows,
    data: Buffer.from(dst.data)
  })
  .write('output.png');
  src.delete();
  dst.delete();
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
        return new Promise((resolve, reject) => {

        resolve("100%")
    })
}
};
