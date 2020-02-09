import Cookies from 'js-cookie';
import userAPI from './userAPI';
import axios from 'axios';

export const authenticateUser = () => {
    const cookie = Cookies.get('userId');
    console.log(cookie);
    return  userAPI.authenticateUser();
  }
  
export const logOut = () => {

  // TO-DO: call logout route, on response, remove the cookie on the front end:
  Object.keys(Cookies.get()).forEach(element => {
    Cookies.remove(element)
  });
}

export const getCpu = () => {
  return axios.get('/api/worker/cpu')
  
}