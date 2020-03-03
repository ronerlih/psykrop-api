import { getTestImages, analyseImage } from "../scripts/imageAnalysis";
import imgDownload from "../scripts/downloadImage";

// Defining methods for the booksController
module.exports = {
    analyseUrls: async function(req, res) {
        const urls = req.body.images;
        let analysisPromisesArray = [];
        urls.forEach((url, id) => {
            analysisPromisesArray.push(analyseImage(url, id));
        });

        Promise.all(analysisPromisesArray)
            .then(result => {
                //check heap memory
                let used = process.memoryUsage();
                for (let key in used) {
                    console.log(`${key} ${Math.round((used[key] / 1024 / 1024) * 100) / 100} MB`);
                }
                console.log(`The script uses approximately ${Math.round(used.heapUsed  / 1024 / 1024 ) } MB`);
                res.status(200).json(result);
            })
            .catch(e => {throw e});
    }
};
