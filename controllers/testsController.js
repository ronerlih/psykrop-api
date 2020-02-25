import { getTestImages, analyseImage } from "../scripts/imageAnalysis";
import imgDownload from "../scripts/downloadImage";

// Defining methods for the booksController
module.exports = {
    runAll: function(req, res) {
        let downloadsPromisesArray = [];
        const urls = getTestImages();
        urls.forEach(async (url, i) => {
            downloadsPromisesArray.push( imgDownload(url, i));
        });

        Promise.all(downloadsPromisesArray)
        .then(images => {
            let analysisPromisesArray = [];
            images.forEach((img, id) => {
                // TO-DO: fix async
                analysisPromisesArray.push(analyseImage(img, id));
            });

            Promise.all(analysisPromisesArray).then(result => {
                 //check heap memory
                 let used = process.memoryUsage();
                 for (let key in used) {
                     console.log(`${key} ${Math.round(used[key] / 1024 / 1024 * 100) / 100} MB`);
                   }
                 console.log(`The script uses approximately ${Math.round(used.heapUsed * 100) / 100} MB`);
 
                res.status(200).json(result);
            });
        })
        .catch(e => {throw e})
    }
};
