const db = require("../models");

// Defining methods for the booksController
module.exports = {
   create: function(req, res) {
      console.log(req.body);
      //validate request
      if (req.body.email && req.body.username && req.body.password && req.body.passwordConf) {
         //create data
         const userData = {
            email: req.body.email,
            username: req.body.username,
            password: req.body.password,
         };
         db.User.create(userData)
            .then((dbModel) => {
               // set the session userid
               req.session.user = dbModel;
               return res.json(dbModel);
            })
            .catch((err) => res.status(422).json(err));
      }
   },

   login: function(req, res, next) {
      console.log("login");

      //validate request
      if (req.body.email && req.body.password) {
         console.log(req.body.email, req.body.password);
         db.User.authenticate(req.body.email, req.body.password, function(error, user) {
            if (error || !user) {
               var err = new Error("Wrong email or password.");
               err.status = 401;
               return next(err);
            } else {
               console.log("login: ", user._id);
               // req.session.userId = user._id;
               req.session.user = user;
               console.log("redirect");
               return res.redirect("/books");
            }
         });
      } else {
         var err = new Error("All fields required.");
         err.status = 400;
         return next(err);
      }
   },

   authenticate: function(req, res) {
      console.log("\n\n\n\nsession.user: ");
      console.log(req.session.user);
      // check only on user - no db call
      return req.session.user ? res.status(200).json(req.session.user) : req.session.destroy(), res.status(201).json("Not authorized! Please sign in.");
   },
   cpu: function(req, res) {
      console.log("get cpou route");
      console.log(process.env.cpuCore);
      res.status(200).json(process.env.cpuCore);
   },
};
