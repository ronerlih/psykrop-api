const router = require("express").Router();
const apiRoutes = require("./api");
const imgRoutes = require("./images");

// API Routes
router.use("/api", apiRoutes);

// If no API routes are hit, send the React app
// router.use("/", function(req, res) {
//   console.log("serving react");
//   res.sendFile(path.join(__dirname, "../client/build/index.html"));
// });
router.use("/images", imgRoutes);


module.exports = router;
