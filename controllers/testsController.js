import { getTestImages, analyseImage } from "../scripts/imageAnalysis";
import imgDownload from "../scripts/downloadImage";

// Defining methods for the booksController
module.exports = {
    runAll: function(req, res) {
        let downloadsPromisesArray = [];
        const urls = getTestImages();
        urls.forEach(url => {
            downloadsPromisesArray.push(imgDownload(url));
        });

        Promise.all(downloadsPromisesArray)
        .then(images => {
            let analysisPromisesArray = [];
            images.forEach(img => {
                // TO-DO: fix async
                analysisPromisesArray.push(setTimeout(function(){ analyseImage(img)}, 1000));
            });

            Promise.all(analysisPromisesArray).then(result => {
                console.log(result[0]);
                res.status(200).json("tests completed");
            });
        });
    }
};
