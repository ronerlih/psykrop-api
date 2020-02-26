import fs from "fs";
import axios from "axios";
import Path from "path";
import "babel-polyfill";

export default async function downloadImage(url, id) {
    return new Promise(async (resolve, reject) => {
        // const now = new Date().toISOString();
            const path = Path.resolve(__dirname, "../images", id + "img.jpg");
            const writer = fs.createWriteStream(path);
            try 
            {
              const response = await axios({
                  url,
                  method: "GET",
                  responseType: "stream"
              });
  
              response.data.pipe(writer);

            } 
            catch (e) {
              console.log('\n\n\n--err-r-');
              // reject(e);
              reject(e);
            }

            writer.on("finish", () => resolve(id + "img.jpg"));
            writer.on("error", () => reject(e));
       
    });
}
