const router = require("express").Router();
const userController = require("../../controllers/userController");

// Matches with "/api/user/signup"
router.route("/signup")
  .post(userController.create);
  
// Matches with "/api/user/authenticate"
router.route("/authenticate")
  .post(userController.authenticate);
  

// Matches with "/api/user/cpu"
router.route("/cpu")
.get(userController.cpu);

// Matches with "/api/user"
router.route("/")
  .post(userController.login);

// TO-DO: logout route (delete cookie and session - req.session.destroy)
module.exports = router;
