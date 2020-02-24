const router = require("express").Router();
const imgController = require("../../controllers/imgController");

console.log('img routes');
// Matches with "/api/tests"
router.route("/:img")
  .get(imgController.getImg)

module.exports = router;
