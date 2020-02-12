import cv from "opencv4nodejs";

module.exports = {
    getTestImages: function() {
        return ["https://i.ytimg.com/vi/MPV2METPeJU/maxresdefault.jpg"];
    },
    analyseImage: function(image) {
        return new Promise((resolve, reject) => {
            // TO-DO: add promisses array from promies.all
            image;
            
            resolve("100%");

            // reject(e);
        });
    }
};
