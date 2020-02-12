import {getTestImages, analyseImages} from "../scripts/imageAnalysis"
// Defining methods for the booksController
module.exports = {
  runAll : function (req, res) {
    const urls = getTestImages();
    const result = analyseImages(urls);
    console.log(result);
      res.status(200).json('tests completed, ' + result)
  }
};
