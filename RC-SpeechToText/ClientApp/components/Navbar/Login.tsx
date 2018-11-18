import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { GoogleLogin } from 'react-google-login';

interface State {

}

export default class Login extends React.Component<any, State> {

  constructor(props: RouteComponentProps<{}>) {
    super(props)
  } 

  public onLoginSuccess = () => {
    alert("Logged in");
  }

  public onLoginFailure = () => {
    alert("Failed log in")
  }

  public render() {
    return(
      <div>
        <GoogleLogin
          className="button is-danger"
          clientId="608596289285-2ap5igg0pluo10sb16pvkbd3ubhdql7h.apps.googleusercontent.com"
          buttonText="Login"
          onSuccess={this.onLoginSuccess}
          onFailure={this.onLoginFailure}
        />
      </div>
    )
  }
}