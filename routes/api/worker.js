const router = require("express").Router();
const workerController = require("../../controllers/workerController");

// Matches with "/api/worker/cpu"
router.route("/cpu")
.get(workerController.cpu);


// TO-DO: logout route (delete cookie and session - req.session.destroy)
module.exports = router;
