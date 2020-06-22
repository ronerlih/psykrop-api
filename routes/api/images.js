const router = require("express").Router();
const apiImagesController = require("../../controllers/apiImagesController");

// Matches with "/api/images"
router.route("/")
   .post(apiImagesController.analyseUrls);

module.exports = router;
