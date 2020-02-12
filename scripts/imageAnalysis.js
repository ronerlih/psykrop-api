const cv = require("opencv4nodejs");
import Path from "path";

module.exports = {
    getTestImages: function() {
        return ["https://i.ytimg.com/vi/MPV2METPeJU/maxresdefault.jpg"];
    },
    analyseImage: function(image) {
        return new Promise((resolve, reject) => {
            const path = Path.resolve(__dirname, "../images/", image);
            console.log(__dirname);

            cv.imreadAsync("./images/img.jpg", (err, mat) => {

                // mat = mat.bitwiseNot();
                cv.imshow("window", mat);
                cv.waitKey();
                resolve("100%");
            })
       

        });
    }
};
