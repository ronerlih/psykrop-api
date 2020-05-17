const router = require("express").Router();
const imgController = require("../../controllers/imgController");

// Matches with "/api/tests"
router.route("/:img")
  .get(imgController.getImg)

module.exports = router;
