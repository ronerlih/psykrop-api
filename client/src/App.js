import React from "react";
import { StaticRouter, BrowserRouter as Router, Route, Switch/*, Redirect*/ } from "react-router-dom";
import Landing from "./pages/Landing";
// import Detail from "./pages/Detail";
// import Login from "./pages/Login";
// import Signup from "./pages/Signup";
import NoMatch from "./pages/NoMatch";
import Nav from "./components/reusable/Nav";
import Alert from "./components/reusable/Alert";
// import {/* getCookie, */ authenticateUser, getCpu } from "./utils/handleSessions";

// const PrivateRoute = ({ component: Component, state: state, ...rest }) => (
//   <Route {...rest} render={(props) => (
//     state.authenticated === true
//       ? <Component {...props} />
//       : state.loading === true
//         ? <div>
//         </div>
//         : <Redirect to='/' />
//   )} />
// )

class App extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      authenticated: false,
      loading: false,
      ssr: props.ssr ? true : false
    }
  }


  // authenticate = () => authenticateUser()
  //   .then(auth => {
  //     console.log("auth.status");
  //     console.log(auth.status);
  //     this.setState({ authenticated: auth.status === 200 ? true : false, loading: false })
  //   })
  //   .catch(err => {
  //     if(process.env.NODE_ENV !== 'production')
  //     console.log(err)
  //   })

  // getCpu = () => getCpu()
  //   .then(cpu => this.setState({ cpu: cpu }))
  //   .catch(err => {
  //     if(process.env.NODE_ENV !== 'production')
  //     console.log(err)
  //   })

  // removeInfo = () => this.setState({ cpu: null })

  componentWillMount() {
      // this.authenticate();
      // this.getCpu();
  }

  render() {
    const RouterComponent = this.state.ssr ? StaticRouter : Router;
    return (
      <RouterComponent>
        <div>
          <Nav />
          <Switch>
            <Route
              exact
              path="/"
              render={(props) =>
                // <Landing {...props} authenticate={this.authenticate} authenticated={this.state.authenticated} />}
                <Landing {...props} />}
            />
            {/* <Route
              exact
              path="/signup"
              render={(props) =>
                <Signup {...props} authenticate={this.authenticate} authenticated={this.state.authenticated} />}
            /> */}
            <Route component={NoMatch} />
          </Switch>
          {/* {this.state.cpu
            ? ""
            : ""
          } */}
          {/* <Alert cpu= {this.state.cpu ? this.state.cpu.data: ""} onclick={this.removeInfo}> */}
              {/* </Alert> */}
              {/* style rollout */}
              {/* <div className="footer bg-dark" style={{opacity: 0.85,borderTop: "10px solid #7ec6a8",position:"relative",height:50}} >
                <span style={{padding:"2px 2px 2px 4px",color:"white",position:"absolute", bottom:0, fontSize:10}}>@devnow</span>
              </div> */}
        </div>
      </RouterComponent>

    )
  }
}

export default App;
