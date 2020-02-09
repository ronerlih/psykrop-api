import axios from "axios";

const xhrHeader = {
  headers: {
    xhrFields: {
      withCredentials: true
    }
  }
};

export default {
  loginUser: function (user) {
    return axios.post("/api/user/", user, xhrHeader)
  },
  signup: function (user) {
    return axios.post("/api/user/signup", user, xhrHeader)
  },
  authenticateUser: function () {
    return axios.post("/api/user/authenticate/", xhrHeader)
  }

};
