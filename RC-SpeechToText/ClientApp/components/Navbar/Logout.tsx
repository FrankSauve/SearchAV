import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { GoogleLogout } from 'react-google-login';

interface State {

}

export default class Login extends React.Component<any, State> {

  constructor(props: RouteComponentProps<{}>) {
    super(props)
  } 

  public onLogout = () => {
    alert("Logged out");
  }

  public render() {
    return(
      <div>
        <GoogleLogout
          className="button is-light"
          buttonText="Logout"
          onLogoutSuccess={this.onLogout}
        >
        </GoogleLogout>
      </div>
    )
  }
}