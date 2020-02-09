import fs from 'fs';
import path from 'path';
import React from 'react';
import ReactDOMServer from 'react-dom/server';
import App from '../client/src/App';
import client from '../scripts/redis';

module.exports = {
  serverRenderer: (req, res, next) => {
    // cache try
    client.get(`ssr:app`, (error, result) => {
      if (error) {
        console.log(error);
      }
      // If that key exist in Redis store
      if (result) {
        console.log("serving from cache");
        return res.send(result);
      } else {
        // Key does not exist in Redis store
        // Fetch from DB (or axios call to external api )
        fs.readFile(path.resolve('./client/build/index.html'), 'utf8', (err, data) => {
          if (err) {
            console.error(err)
            return res.status(500).send('An error occurred')
          }
          console.log("\n\n\nssr from fs")
          let appComponent = ReactDOMServer.renderToString(<App ssr/>)

          try {
            client.setex(`ssr:app`, 1000 * 60 * 60 * 24, data.replace(
              '<div id="root"></div>',
              `<div id="root">${appComponent}</div>`
            ));
          }
          catch(err){
            console.log('\n\n\n\nredit write err');
            console.log(err);
          }
          
          return res.set("Content-Type", "text/html").send(
            data.replace(
              '<div id="root"></div>',
              `<div id="root">${appComponent}</div>`
            )
          )
        })
      }
    })
  }
}
