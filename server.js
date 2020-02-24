const express = require("express");
const router = require("express").Router();
const path = require("path");
const morgan = require("morgan");
// const mongoose = require("mongoose");
const APIandAppRoutes = require("./routes");
const app = express();
const PORT = process.env.PORT || 3001;
const compression = require('compression')
// const session = require('express-session')
// connect redis 
// const RedisStore = require('connect-redis')(session)
// import client from './scripts/redis';
// import initSession from './scripts/session';
// import {serverRenderer} from './controllers/serverSideRendering';
import errorHandler from './scripts/errorHandler';

//logs
app.use(morgan("dev"));

// compress responses
app.use(compression())

//use sessions for tracking logins
// connect redis
// app.use(initSession(session, RedisStore, client));
// app.use(initSession(session));

// Define middleware here
app.use(express.urlencoded({ extended: true }));
app.use(express.json());

// ssr
// if (process.argv.indexOf("no-ssr") < 0)
//   router.use("^/$", serverRenderer)



// Add router (ssr and static)
// app.use(router);
// Add API and view routes
app.use(APIandAppRoutes);

// Serve up static assets (usually on heroku)
if (process.env.NODE_ENV === "production") {
  app.use(express.static(path.resolve(__dirname, 'client/build'), { maxAge: '30d' }));
 }
 
// error handling
app.use((err, req, res, next) => errorHandler(err, req, res, next));

// // Connect to the Mongo DB
// mongoose.connect(process.env.MONGODB_URI || "mongodb://localhost/mern-auth",{   useCreateIndex: true,
// useUnifiedTopology: true, useNewUrlParser: true});

// Start the API server
app.listen(PORT, function () {
  console.log(`🌎  ==> API Server now listening on PORT ${PORT}!  ${process.env.cpuCore ? "on CPU " + process.env.cpuCore : "" }`);
});
