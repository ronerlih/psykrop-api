import { analyseImage } from "../scripts/imageAnalysis";

// Defining methods for the booksController
module.exports = {
    analyseUrls: async function(req, res) {
        let used = process.memoryUsage();
        console.log(`Heap used before analysis: ${(used.heapUsed / 1024 / 1024).toFixed(2)} MB`);

        const urls = req.body.images;
        let analysisPromisesArray = [];
        urls.forEach((url, id) => {
            analysisPromisesArray.push(analyseImage(url, id));
        });

        Promise.all(analysisPromisesArray)
            .then(result => {
                //check heap memory
                let used = process.memoryUsage();
                // for (let key in used) {
                //     console.log(`${key} ${Math.round((used[key] / 1024 / 1024) * 100) / 100} MB`);
                // }
                console.log(`Heap used: ${(used.heapUsed / 1024 / 1024).toFixed(2)}MB`);
                // sort by decending
                result.sort((a,b) =>  b.balanceAllCoefficients - a.balanceAllCoefficients );

                res.status(200).json(result);
            })
            .catch(e => {throw e});
    }
};
