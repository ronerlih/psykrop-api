import cv from "opencv4nodejs";
import imgDownload from "./downloadImage";

module.exports = {
    getTestImages: function(){
        return ['https://i.ytimg.com/vi/MPV2METPeJU/maxresdefault.jpg']
    },
    analyseImages: function(images){
        images.forEach(url => {
            imgDownload(url); 
        });
        return "true"
    }
}