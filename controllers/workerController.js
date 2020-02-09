
// Defining methods for the workerController
module.exports = {

  cpu: function (req, res, next){
    console.log("get cpou route");
    console.log(process.env.cpuCore);
    res.status(200).json(process.env.cpuCore)
  }
};
