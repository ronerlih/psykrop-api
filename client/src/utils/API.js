import axios from "axios";

export default {
  runTests: function(){
    return axios.get("/api/tests");
    
  },
  callPost: function(urls){
    const urlArray = urls ? urls.split(","): [""]
    return axios.post("/api/images",{images:urlArray});
    
  },
  // Gets all books
  getBooks: function () {
    return axios.get("/api/books");
  },
  // Gets the book with the given id
  getBook: function (id) {
    return axios.get("/api/books/" + id);
  },
  // Deletes the book with the given id
  deleteBook: function (id) {
    return axios.delete("/api/books/" + id);
  },
  // Saves a book to the database
  saveBook: function (bookData) {
    return axios.post("/api/books", bookData);
  }

};
