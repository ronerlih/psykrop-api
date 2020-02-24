import { getTestImages, analyseImage } from "../scripts/imageAnalysis";
import imgDownload from "../scripts/downloadImage";

// Defining methods for the booksController
module.exports = {
    runAll: function(req, res) {
        let downloadsPromisesArray = [];
        const urls = getTestImages();
        urls.forEach(async url => {
            downloadsPromisesArray.push( imgDownload(url));
        });

        Promise.all(downloadsPromisesArray)
        .then(images => {
            let analysisPromisesArray = [];
            images.forEach((img, id) => {
                // TO-DO: fix async
                analysisPromisesArray.push(analyseImage(img, id));
            });

            Promise.all(analysisPromisesArray).then(result => {
                res.status(200).json(result[0]);
            });
        });
    }
};
