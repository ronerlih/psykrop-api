import {getTestImages, analyseImages} from "../scripts/imageAnalysis"
// Defining methods for the booksController
module.exports = {
  runAll : function (req, res) {
    const urls = getTestImages();
    analyseImages(urls)
      .then(result => {
    console.log("aftr err");

        console.log(result)
        res.status(200).json('tests completed, ' + result)});
      
  }
};
