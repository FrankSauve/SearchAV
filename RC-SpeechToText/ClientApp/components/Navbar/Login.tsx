import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { GoogleLogin } from 'react-google-login';
import { Redirect } from 'react-router-dom';
import auth from '../../Utils/auth';

interface State {
    redirectToVideos: boolean
}

export default class Login extends React.Component<any, State> {

    constructor(props: RouteComponentProps<{}>) {
        super(props);

        this.state = {
            redirectToVideos: false
        }
    }

    public onLoginSuccess = (response:any) => {
        console.log("Google login response: " + response);
        auth.setAuthToken(response.tokenId);
        // TODO: Make request to server
        this.setState({'redirectToVideos': true})
    };

    public onLoginFailure = (response: any) => {
        console.log("Google login response: " + response);
        alert("Failed login")
    };

    public render() {
        return (
            <div>
                <GoogleLogin
                    className="button is-danger"
                    clientId="608596289285-2ap5igg0pluo10sb16pvkbd3ubhdql7h.apps.googleusercontent.com"
                    buttonText="Login"
                    onSuccess={this.onLoginSuccess}
                    onFailure={this.onLoginFailure}
                />
                {this.state.redirectToVideos ? <Redirect to="/videos"/> : null}
            </div>
        )
    }
}