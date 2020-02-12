import fs from "fs";
import axios from "axios";
import Path from "path";
import 'babel-polyfill';


export default async function downloadImage (url) {  
        const now = new Date().toISOString();
        const path = Path.resolve(__dirname, '../images', 'img.jpg')
        const writer = fs.createWriteStream(path)
      
        const response = await axios({
          url,
          method: 'GET',
          responseType: 'stream'
        })
      
        response.data.pipe(writer)
      
        return new Promise((resolve, reject) => {
          writer.on('finish', resolve('img.jpg'))
          writer.on('error', reject(e))
        })
      }
      