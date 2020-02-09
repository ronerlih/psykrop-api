export default function (err, req, res, next) {
  console.log("\n\n\n\n---\nerr handler called");
    if (res.headersSent) {
      console.log('error sent');
      return next(err)
    }
    console.log('error will send');
    res.status(500)
    res.send(`server error:<br>error msg: ${err}`)
  }