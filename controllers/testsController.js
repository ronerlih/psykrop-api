import { getTestImages, analyseImage } from "../scripts/imageAnalysis";
let analysisPromisesArray = [];
const urls = getTestImages();
let used;

// Defining methods for the booksController
module.exports = {
    analyseForFrontEndTestDashboard: async function(req, res) {
        
        used = process.memoryUsage();
        console.log(`Heap before analysis:`);
        for (let key in used) {
            console.log(`${key} ${Math.round(used[key] / 1024 / 1024)}MB`);
          }
            urls.forEach((img, id) => {
                analysisPromisesArray.push(analyseImage(img, id, true).catch(e => {throw e}));
            });
            Promise.all(analysisPromisesArray).then(result => {
                 //check heap memory
                used = process.memoryUsage();
                 for (let key in used) {
                     console.log(`${key} ${Math.round(used[key] / 1024 / 1024 * 100) / 100} MB`);
                   }
                console.log(`Heap used: ${used.heapUsed / 1024 / 1024} MB`);
                
                 res.status(200).json(result);
                //  setTimeout(() => process.exit(0), 10)
            })
        .catch(e => {throw e})

    }
};
