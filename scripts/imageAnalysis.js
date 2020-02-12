import cv from "opencv4nodejs";
import imgDownload from "./downloadImage";

module.exports = {
    getTestImages: function(){
        return ['https://i.ytimg.com/vi/MPV2METPeJU/maxresdefault.jpg']
    },
    analyseImages: function(images){
        return new Promise((resolve, reject) => {
            images.forEach(url => {
                imgDownload(url)
                   .then(imgPath => {
                       resolve("100%")
                   })
                   .catch(e => {
                       console.log(e);
                       reject(e);
                   });
           });
        })

    }
}