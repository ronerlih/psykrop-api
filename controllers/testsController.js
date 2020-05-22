import OpenCV from "../scripts/imageAnalysis";

// Defining methods for the booksController
module.exports = {
   analyseForFrontEndTestDashboard: async function(req, res) {
      let analysisPromisesArray = [];
      const urls = OpenCV.getTestImages();
      let used;
      used = process.memoryUsage();
      console.log("Heap before analysis:");
      for (let key in used) {
         console.log(`${key} ${Math.round(used[key] / 1024 / 1024)}MB`);
      }
      urls.forEach((img, id) => {
         analysisPromisesArray.push(OpenCV.analyseImage(img, id, true));
      });
      Promise.all(analysisPromisesArray)
         .then((result) => {
            //check heap memory
            used = process.memoryUsage();
            for (let key in used) {
               console.log(`${key} ${Math.round((used[key] / 1024 / 1024) * 100) / 100} MB`);
            }
            console.log(`Heap used: ${used.heapUsed / 1024 / 1024} MB`);

            res.status(200).json(result);
            //  setTimeout(() => process.exit(0), 10)
         })
         .catch((e) => {
            throw e;
         });
   },
};
