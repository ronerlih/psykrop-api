var path = require('path');

// Defining methods for the booksController
module.exports = {
    getImg: function(req, res) {
            // console.log(__dirname + '/../images/' + req.params.img);
            res.sendFile(path.resolve(__dirname + '/../images/' + req.params.img));
        }
};
