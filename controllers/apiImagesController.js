import { analyseImage } from "../scripts/imageAnalysis";

// Defining methods for the booksController
module.exports = {
   analyseUrls: async function(req, res) {
      let used = process.memoryUsage();
      console.log(`Heap used before analysis: ${(used.heapUsed / 1024 / 1024).toFixed(2)} MB`);
      const analysisPromisesArray = [];
      console.log((req.body));

      if (!req.body || !Array.isArray(req.body)) {
         res.status(403).send({ Error: "no images array in body, please compose your request with an array of image urls in the body (application/json contentType) or comma seperated urls (form/urlencoded contentType)." });
      } else {
         try {
            req.body.forEach((url, id) => {
               analysisPromisesArray.push(analyseImage(url, id));
            });
         } catch (e) {
            throw e;
         }

         Promise.all(analysisPromisesArray)
            .then((result) => {
               //check heap memory
               let used = process.memoryUsage();
               // for (let key in used) {
               //     console.log(`${key} ${Math.round((used[key] / 1024 / 1024) * 100) / 100} MB`);
               // }
               console.log(`Heap used: ${(used.heapUsed / 1024 / 1024).toFixed(2)}MB`);

               // order results
               if (req.query.order === "desc") {
                  // order by decending
                  console.log("descending option");
                  result.sort((a, b) => b.balanceAllCoefficients - a.balanceAllCoefficients);
               } else if (req.query.order === "asc") {
                  // order by decending
                  console.log("ascending option");
                  result.sort((a, b) => a.balanceAllCoefficients - b.balanceAllCoefficients);
               }
               res.status(200).json(result);
            })
            .catch((e) => {
               console.log("\n\n\n\nerror", e);
               // throw e;
            });
      }
   },
};
