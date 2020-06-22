const router = require("express").Router();
// const bookRoutes = require("./books");
// const userRoutes = require("./user");
const workerRoutes = require("./worker");
const imagesRoutes = require("./images");
const testsRoutes = require("./visualTests");
// const redisClient = require('../../scripts/redis');

// console.log(redisClient)
// /api/book routes
// router.use("/books", bookRoutes);

// /api/user routes
// router.use("/user", userRoutes);

// /api/worker routes
router.use("/worker", workerRoutes);

// /api/images routes
router.use("/images", imagesRoutes);

// /api/tests routes
router.use("/visualtests", testsRoutes);

module.exports = router;
