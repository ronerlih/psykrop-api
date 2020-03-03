import { getTestImages, analyseImage } from "../scripts/imageAnalysis";
import imgDownload from "../scripts/downloadImage";

// Defining methods for the booksController
module.exports = {
    runAll: async function(req, res) {
        const urls = getTestImages();

            let analysisPromisesArray = [];
            urls.forEach((img, id) => {
                // TO-DO: fix async
                analysisPromisesArray.push(analyseImage(img, id, true));
            });
            console.log(analysisPromisesArray);
            Promise.all(analysisPromisesArray).then(result => {
                 //check heap memory
                 let used = process.memoryUsage();
                 for (let key in used) {
                     console.log(`${key} ${Math.round(used[key] / 1024 / 1024 * 100) / 100} MB`);
                   }
                 console.log(`The script uses approximately ${Math.round(used.heapUsed * 100) / 100} MB`);
                
                 res.status(200).json(result);
            })
        .catch(e => {throw e})

         function downloadImages(){
            const downloadsPromisesArray = [];
            let urlsLength = urls.length;
            return new Promise((resolve, reject) => {
                urls.forEach( (url, i) => {
                    urlsLength--;
                    
                    try{
                        downloadsPromisesArray.push(imgDownload(url, i))
                    }
                    catch(e){
                        console.log('\n--error--');
                        console.log(e.message);
                    }

                    if (urlsLength === 0){
                        console.log("downloadsPromisesArray");
                        console.log(downloadsPromisesArray);
                        resolve(downloadsPromisesArray);
                    }
                    
                    
                });
            })

        }
    }
};
