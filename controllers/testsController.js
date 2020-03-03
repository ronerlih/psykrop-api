import { getTestImages, analyseImage } from "../scripts/imageAnalysis";
import imgDownload from "../scripts/downloadImage";

// Defining methods for the booksController
module.exports = {
    analyseForFrontEndTestDashboard: async function(req, res) {
        const urls = getTestImages();
        
        let used = process.memoryUsage();
        console.log(`Heap before analysis:`);
        for (let key in used) {
            console.log(`${key} ${Math.round(used[key] / 1024 / 1024)}MB`);
          }
            let analysisPromisesArray = [];
            urls.forEach((img, id) => {
                analysisPromisesArray.push(analyseImage(img, id, true));
            });
            Promise.all(analysisPromisesArray).then(result => {
                 //check heap memory
                 let used = process.memoryUsage();
                 for (let key in used) {
                     console.log(`${key} ${Math.round(used[key] / 1024 / 1024 * 100) / 100} MB`);
                   }
                console.log(`Heap used: ${used.heapUsed / 1024 / 1024} MB`);
                
                 res.status(200).json(result);
            })
        .catch(e => {throw e})

    }
};
