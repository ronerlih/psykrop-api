const router = require("express").Router();
const testsController = require("../../controllers/testsController");

// Matches with "/api/tests"
router.route("/")
  .get(testsController.analyseForFrontEndTestDashboard)

module.exports = router;
